using _Game.Common;
using _Game.Core._GameMode;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;
using _Game.Gameplay.Common;
using _Game.Utils.Timers;
using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace _Game.Core.Ads.ApplovinMaxAds
{
    public class MaxAdsService : IAdsService, IDisposable
    {

#if UNITY_IOS
        private readonly string _interstitialID = "15f35bbfd0f966ff";
        private readonly string _rewardedID = "d1fd0c994dee3e7d";

#elif UNITY_ANDROID
        private readonly string _interstitialID = "a1e8dd54aafa9d07";
        private readonly string _rewardedID = "a670b96811ca143e";
#endif

        public event Action RewardedVideoLoaded;
        public event Action InterstitialVideoLoaded;
        public event Action<int> OnCountAds;
        public event Action<string, MaxSdkBase.AdInfo> OnAdRevenuePaidEvent;
        public event Action<string, MaxSdkBase.AdInfo, Placement> OnAdRevenueWPlacementEvent;
        public event Action<AdType, Placement, MaxSdkBase.AdInfo, AdStatus, int> OnAdImpressionStatus;
        public event Action<AdType, Placement, MaxSdkBase.AdInfo, int> OnAdImpressionCustom;

        private Action _onVideoCompleted;
        private Placement _placement;
        private readonly IMyLogger _logger;
        private readonly IAdsConfigRepository _adsConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IGameManager _gameManager;

        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;

        private bool IsTimeForInterstitial { get; set; }

        private int _rewardedRetryAttempt;
        private int _interstitialRetryAttempt;

        private SynchronizedCountdownTimer _countdownTimer;

        private CancellationTokenSource _interstitialCts;

        public bool CanShowInterstitial =>
            _adsConfigRepository.GetConfig().IsInterstitialActive &&
            (Purchases.BoughtIAPs?.Find(x => x.Count > 0) == null) &&
            IsInternetConnected() &&
            BattleStatistics.BattlesCompleted > _adsConfigRepository.GetConfig().InterstitialBattleTreshold;

        public MaxAdsService(
            IMyLogger logger,
            IUserContainer userContainer,
            IConfigRepository configRepository,
            IGameManager gameManager
        )
        {
            _logger = logger;
            _adsConfigRepository = configRepository.AdsConfigRepository;
            _userContainer = userContainer;
            _gameManager = gameManager;
        }

        public void Init()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializeRewardedAds();
                InitializeInterstitialAds();
            };
            Debug.Log("MaxSdk.InitializeSdk");

            MaxSdk.InitializeSdk();

            LoadAndShowCmpFlow();

            Subscribe();
            Debug.Log("MaxSdk.Done");

            OnInterstitialAdTimerOut();
        }

        void IDisposable.Dispose()
        {
            Unsubscribe();
        }

        public bool IsAdReady(AdType type)
        {
#if UNITY_EDITOR
            //Mock
            //return false;
            return true;
#endif
            switch (type)
            {
                case AdType.Rewarded:
                    return MaxSdk.IsRewardedAdReady(_rewardedID);

                case AdType.Interstitial:
                    return MaxSdk.IsInterstitialReady(_interstitialID);

                default:
                    return false;
            }
        }

        public void ShowInterstitialVideo(Placement placement)
        {
            _logger.Log("Inter_ ShowInterstitialVideo");
            if (IsTimeForInterstitial && CanShowInterstitial)
            {
                _logger.Log("Inter_ Can Show Ready");
                _placement = placement;

                var delay = _adsConfigRepository.GetConfig().InterstitialDelay;
                MaxSdk.ShowInterstitial(_interstitialID);
                StartCountdown(delay);

                _userContainer.State.AdsStatistics.AddWatchedAd();
                OnCountAds?.Invoke(_userContainer.State.AdsStatistics.AdsReviewed);
                _userContainer.State.AdsWeeklyWatchState.AddWatchedAd(DateTime.UtcNow);
            }
            else
            {
                _logger.Log("Inter_ Can't Show Inter");
            }
        }

        public void ShowRewardedVideo(Action onVideoCompleted, Placement placement)
        {
            if (IsAdReady(AdType.Rewarded))
            {
                _logger.Log("Rewarded Video Show");
                _onVideoCompleted = onVideoCompleted;
                _placement = placement;
                var delay = _adsConfigRepository.GetConfig().RewardInterstitialDelay;
                StartCountdown(delay);
                MaxSdk.ShowRewardedAd(_rewardedID);
                _userContainer.State.AdsStatistics.AddWatchedAd();
                _userContainer.State.AdsWeeklyWatchState.AddWatchedAd(DateTime.UtcNow);
                OnCountAds?.Invoke(_userContainer.State.AdsStatistics.AdsReviewed);
                _gameManager.SetPaused(true);
            }
            else
            {
                Debug.Log("Ad not ready");
            }
        }

        private void LoadAndShowCmpFlow()
        {
            var cmpService = MaxSdk.CmpService;

            cmpService.ShowCmpForExistingUser(error =>
            {
                if (null == error)
                {
                    // The CMP alert was shown successfully.
                }
            });
        }

        private void Subscribe()
        {
            if (GameModeSettings.I.IsTestAds)
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
                {
                    MaxSdk.ShowMediationDebugger();
                };
            }
        }

        private void Unsubscribe()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRevenuePaid;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= OnRevenuePaid;
        }

        private void StartCountdown(float delaySeconds)
        {
            _logger.Log($"[Ad] START INTERSTITIAL COUNTDOWN: {delaySeconds} ");

            //     
            IsTimeForInterstitial = false;

            //   ,  
            _interstitialCts?.Cancel();
            _interstitialCts = new CancellationTokenSource();

            //   
            _ = RunInterstitialCountdownAsync(delaySeconds, _interstitialCts.Token);
        }

        private async UniTask RunInterstitialCountdownAsync(float delaySeconds, CancellationToken token)
        {
            float elapsed = 0f;
            float interval = 0.1f;

            while (elapsed < delaySeconds)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);

                if (token.IsCancellationRequested)
                    return;

                if (Application.isFocused)
                {
                    elapsed += interval;
                }
            }

            IsTimeForInterstitial = true;
            _logger.Log($"[Ad] INTERSTITIAL READY: {IsTimeForInterstitial}");
        }


        //private void StartCountdown(float delay)
        //{
        //    _logger.Log($"START INTERSTITIAL COUNTDOWN! {delay}", DebugStatus.Warning);

        //    if (_countdownTimer != null)
        //    {
        //        _countdownTimer.Stop();
        //        IsTimeForInterstitial = false;
        //    }


        //    if (_countdownTimer == null)
        //    {
        //        _countdownTimer = new SynchronizedCountdownTimer(delay);
        //        _countdownTimer.TimerStop += OnInterstitialAdTimerOut;
        //    }

        //    _countdownTimer.Start();

        //    _logger.Log($"INTERSTITIAL READY: {IsTimeForInterstitial}!", DebugStatus.Warning);
        //}

        private bool IsInternetConnected() =>
            Application.internetReachability != NetworkReachability.NotReachable;

        private void OnInterstitialAdTimerOut()
        {
            IsTimeForInterstitial = true;
        }

        private async UniTask RetryLoadWithDelay(Func<UniTask> loadFunc, double retryDelay)
        {
            _logger.Log($"Retrying in {retryDelay} seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(retryDelay));
            await loadFunc();
        }

        private void OnRevenuePaid(string arg1, MaxSdkBase.AdInfo arg2)
        {
            OnAdRevenuePaidEvent?.Invoke(arg1, arg2);
            OnAdRevenueWPlacementEvent?.Invoke(arg1, arg2, _placement);
        }

        #region Rewarded Ad Methods

        private async void InitializeRewardedAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRevenuePaid;

            await LoadRewardedAd();
        }

        public async UniTask LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_rewardedID);
            _logger.Log("Rewarded ad loading...", DebugStatus.Success);
            await UniTask.CompletedTask;
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad loaded", DebugStatus.Success);
            RewardedVideoLoaded?.Invoke();
            _rewardedRetryAttempt = 0;
        }

        private async void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));

            _logger.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            await RetryLoadWithDelay(LoadRewardedAd, retryDelay);
        }

        private async void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            await LoadRewardedAd();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad displayed");
            OnAdImpressionStatus?.Invoke(AdType.Rewarded, _placement, adInfo, AdStatus.Show, _userContainer.State.AdsStatistics.AdsReviewed);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad clicked");
            OnAdImpressionStatus?.Invoke(AdType.Rewarded, _placement, adInfo, AdStatus.Click, _userContainer.State.AdsStatistics.AdsReviewed);
        }

        private async void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad dismissed");
            OnAdImpressionStatus?.Invoke(AdType.Rewarded, _placement, adInfo, AdStatus.Close, _userContainer.State.AdsStatistics.AdsReviewed);
            _gameManager.SetPaused(false);
            await LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Rewarded ad received reward");
            _onVideoCompleted?.Invoke();
            _gameManager.SetPaused(false);
            OnAdImpressionStatus?.Invoke(AdType.Rewarded, _placement, adInfo, AdStatus.Complete, _userContainer.State.AdsStatistics.AdsReviewed);

            OnAdImpressionCustom?.Invoke(AdType.Rewarded, _placement, adInfo, _userContainer.State.AdsStatistics.AdsReviewed);
        }

        #endregion


        #region Interstitial Ad Methods

        private async void InitializeInterstitialAds()
        {
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnRevenuePaid;

            await LoadInterstitial();
        }

        private async UniTask LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_interstitialID);
            _logger.Log("Interstitial AD Loading...");
            await UniTask.CompletedTask;
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial loaded", DebugStatus.Success);

            InterstitialVideoLoaded?.Invoke();

            _interstitialRetryAttempt = 0;
        }

        private async void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            _logger.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            await RetryLoadWithDelay(LoadInterstitial, retryDelay);
        }

        private async void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            await LoadInterstitial();
        }

        private async void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _logger.Log("Interstitial dismissed");
            await LoadInterstitial();
        }

        #endregion
    }
}