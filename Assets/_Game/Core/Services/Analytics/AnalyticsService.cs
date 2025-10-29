using System;
using System.Linq;
using _Game.Common;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._GameMode;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Buyer;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common;
using _Game.UI._Skills.Scripts;
using _Game.Utils;
using Assets._Game.Core.UserState;
using DevToDev.Analytics;
using DevToDev.Purchases;
using DTDEditor;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using Zenject;
using Product = UnityEngine.Purchasing.Product;

namespace _Game.Core.Services.Analytics
{
    public class AnalyticsService : IDisposable, IInitializable
    {
        private readonly IUserContainer _userContainer;
        private readonly IAdsService _adsService;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly IIGPService _igpService;
        private readonly IDifficultyConfigRepository _difficulty;
        private readonly IIAPService _iapService;
#if UNITY_IOS
        private readonly IOSTrackIDFAService _iosTrackIDFAService;
        private string _pushStatusTemp;
#endif
        private readonly DTDObjectSettings _settings;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private ICardsCollectionStateReadonly Cards => _userContainer.State.CardsCollectionState;
        private IDailyTasksStateReadonly DailyTasks => _userContainer.State.DailyTasksState;

        private IAdsGemsPackContainer AdsGemsPackContainer => _userContainer.State.AdsGemsPackContainer;
        private IFreeGemsPackContainer FreeGemsPackContainer => _userContainer.State.FreeGemsPackContainer;

        private IDungeonsStateReadonly Dungeons => _userContainer.State.DungeonsSavegame;

        private int TimelineNumber => TimelineState.TimelineId + 1;
        private int AgeNumber => TimelineState.AgeId + 1;
        private int BattleNumber => TimelineState.MaxBattle;

        private readonly TutorialTracker _tutorialTracker;
        private readonly CurrencyTracker _currencyTracker;
        private readonly DungeonsTracker _dungeonsTracker;
        private readonly LevelTracker _levelTracker;
        private readonly CardsTracker _cardsTracker;
        private readonly SkillsTracker _skillsTracker;
        private readonly IAnalyticsEventSender _sender;
        private readonly FbConfigTracker _fbConfigTracker;
        private readonly LocalWaveTraker _wavesTracker;

        public AnalyticsService(
            DTDObjectSettings settings,
            IUserContainer userContainer,
            IAdsService adsService,
            IGameInitializer gameInitializer,
            IMyLogger logger,
            IIGPService igpService,
            IConfigRepository config,
            IIAPService iapService,
            CurrencyBank currencyBank,
            IDungeonModelFactory dungeonsModelFactory,
            IAnalyticsEventSender sender,
            IFeatureUnlockSystem featureUnlockSystem,
            ISkillService skillService,
            IProductBuyer productBuyer

#if UNITY_IOS
           , IOSTrackIDFAService iosTrackIDFAService
#endif
            )
        {
            _sender = sender;
            _userContainer = userContainer;
            _adsService = adsService;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _iapService = iapService;
            _igpService = igpService;
            _settings = settings;
            _difficulty = config.DifficultyConfigRepository;
#if UNITY_IOS
            _iosTrackIDFAService = iosTrackIDFAService;
            _iosTrackIDFAService.OnIDFASendEvent += IDFAStatus;
#endif
            gameInitializer.OnPostInitialization += Subscribe;

            _tutorialTracker = new TutorialTracker(userContainer, sender);
            _currencyTracker = new CurrencyTracker(currencyBank, userContainer, sender);
            _dungeonsTracker = new DungeonsTracker(userContainer, dungeonsModelFactory, config.FeatureConfigRepository, sender);
            _levelTracker = new LevelTracker(userContainer, logger, sender);
            _skillsTracker = new SkillsTracker(userContainer, skillService, featureUnlockSystem, config, logger, sender);
            _cardsTracker = new CardsTracker(userContainer, productBuyer, sender);
            _fbConfigTracker = new FbConfigTracker(sender, config);
            _wavesTracker = new LocalWaveTraker(logger, userContainer);
        }

        void IInitializable.Initialize()
        {
            Initialize();
        }

        private void Initialize()
        {
            var config = new DTDAnalyticsConfiguration
            {
                ApplicationVersion = Application.version,
                LogLevel = _settings.DTDLogLevel,
                TrackingAvailability = DTDTrackingStatus.Enable,
            };

            _logger.Log($"Dev2Dev Start Init");
            DTDAnalytics.SetLogLevel(_settings.DTDLogLevel);

#if UNITY_ANDROID
            InitializeAnalytics(DTDPlatform.Android, config);
#elif UNITY_IOS
            InitializeAnalytics(DTDPlatform.IOS, config);
#endif
            _logger.Log($"Dev2Dev Done Init");

            if (GameModeSettings.I.IsCheatEnabled)
                DTDUserCard.SetTester(true);

            //  DTDPurchases.StartAutoTracking();
        }

        private void InitializeAnalytics(DTDPlatform platform, DTDAnalyticsConfiguration config)
        {
            var targetCredential = _settings.Credentials.FirstOrDefault(item => item.Platform == platform);

            DTDAnalytics.Initialize(targetCredential.Key, config);
            //DTDPurchases.StartAutoTracking();
        }

        private void Subscribe()
        {
            _logger.Log("Dev2Dev INITIALIZED", DebugStatus.Warning);

            TimelineState.NextBattleOpened += OnBattleCompleted;
            TimelineState.LastBattleWon += OnBattleCompleted;
            TimelineState.NextBattleOpened += SetUserDataBattle;

            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            TimelineState.NextTimelineOpened += SetUserDataTimeline;

            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.NextAgeOpened += SetUserDataEvolution;

            TimelineState.OpenedUnit += OnUnitOpened;
            _adsService.OnAdRevenueWPlacementEvent += TrackAdImpression;
            _adsService.OnCountAds += SetUserDataCountAds;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;

            DailyTasks.TaskCompletedChanged += OnDailyTaskCompleted;

            _iapService.Purchased += TrackProfitIap;
            _iapService.OnEventPurchasedDTD += TrackPurchase;
            _adsService.OnAdImpressionCustom += SendCustomAdImpression;

            foreach (var pack in AdsGemsPackContainer.AdsGemsPacks.Values)
            {
                pack.AdsGemsPackCountChanged += TrackAdsGemsPackCount;
            }

            foreach (var pack in FreeGemsPackContainer.FreeGemsPacks.Values)
            {
                pack.FreeGemsPackCountChanged += TrackFreeGemsPackCount;
            }

            _tutorialTracker.Initialize();
            _currencyTracker.Initialize();
            _dungeonsTracker.Initialize();
            _levelTracker.Initialize();
            _cardsTracker.Initialize();
            _skillsTracker.Initialize();
            _fbConfigTracker.Initialize();
            _wavesTracker.Initialize();
        }

        public void Dispose()
        {
            _tutorialTracker.Dispose();
            _currencyTracker.Dispose();
            _dungeonsTracker.Dispose();
            _levelTracker.Dispose();
            _cardsTracker.Dispose();
            _skillsTracker.Dispose();
            _fbConfigTracker.Dispose();
            _wavesTracker.Dispose();

            TimelineState.NextBattleOpened -= OnBattleCompleted;
            TimelineState.LastBattleWon -= OnBattleCompleted;
            TimelineState.NextBattleOpened -= SetUserDataBattle;

            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            TimelineState.NextTimelineOpened -= SetUserDataTimeline;

            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.NextAgeOpened -= SetUserDataEvolution;

            TimelineState.OpenedUnit -= OnUnitOpened;
            _adsService.OnAdRevenueWPlacementEvent -= TrackAdImpression;
            _adsService.OnCountAds -= SetUserDataCountAds;
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleChanged;

            DailyTasks.TaskCompletedChanged -= OnDailyTaskCompleted;

            _iapService.Purchased -= TrackProfitIap;
            _iapService.OnEventPurchasedDTD -= TrackPurchase;
            _adsService.OnAdImpressionCustom -= SendCustomAdImpression;

            foreach (var pack in AdsGemsPackContainer.AdsGemsPacks.Values)
            {
                pack.AdsGemsPackCountChanged -= TrackAdsGemsPackCount;
            }
            foreach (var pack in FreeGemsPackContainer.FreeGemsPacks.Values)
            {
                pack.FreeGemsPackCountChanged -= TrackFreeGemsPackCount;
            }
#if UNITY_IOS
            _iosTrackIDFAService.OnIDFASendEvent -= IDFAStatus;
#endif
            _gameInitializer.OnPostInitialization -= Subscribe;
        }

        private void TrackPurchase(Product product)
        {
            if (product == null || product.metadata == null || string.IsNullOrEmpty(product.transactionID))
            {
                _logger.LogWarning("Invalid product data");
                return;
            }

            string orderId = product.transactionID;
            double price = (double)product.metadata.localizedPrice;
            string productId = product.definition.id;
            string currencyCode = product.metadata.isoCurrencyCode;

            var parameters = new DTDCustomEventParameters();

            parameters.Add("CurrencyName", currencyCode);
            parameters.Add("CurrencyAmount", price);
            parameters.Add("ProductName", productId);
            parameters.Add("Placement", "IAPShop");
            parameters.Add("BattleID", BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", AgeNumber);
            parameters.Add("TimelineID", TimelineNumber);

            _sender.CustomEvent("IAPpurchase", parameters);
            _sender.RealCurrencyPayment(orderId, price, productId, currencyCode);
        }

        public void SendRateUs(string parameter)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("Value", parameter);
            _sender.CustomEvent("rate_us", parameters);
        }
        private void OnDailyTaskCompleted()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("Task №", DailyTasks.CompletedTasks.Count);
            parameters.Add("Task Id", DailyTasks.CompletedTasks.LastOrDefault().ToString());
            parameters.Add("TimelineID", TimelineNumber);
            parameters.Add("AgeID", AgeNumber);
            parameters.Add("BattleID", BattleNumber);

            _sender.CustomEvent("daily_task_completed", parameters);
        }

        private void OnNextAgeOpened()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineNumber);
            parameters.Add("age№", AgeNumber);
            _sender.CustomEvent("evolution_completed", parameters);

            OnUnitOpened(UnitType.Light);
        }

        private void OnUnitOpened(UnitType type)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("timeline№", TimelineNumber);
            parameters.Add("age№", AgeNumber);
            parameters.Add("unit", (int)type);

            _sender.CustomEvent("unit_opened", parameters);
        }
        private void OnBattleCompleted()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("TimelineID", TimelineNumber);
            parameters.Add("AgeID", (TimelineState.AgeId + 1));
            parameters.Add("BattleID", TimelineState.MaxBattle);

            _sender.CustomEvent("battle_completed", parameters);
        }

        private void OnNextTimelineOpened()
        {
            var parametersOpened = new DTDCustomEventParameters();
            parametersOpened.Add("TimelineID", TimelineNumber);
            parametersOpened.Add("AgeID", AgeNumber);
            parametersOpened.Add("BattleID", TimelineState.MaxBattle + 1);
            _sender.CustomEvent("timeline_opened", parametersOpened);


            var parametersFinished = new DTDCustomEventParameters();
            parametersFinished.Add("TimelineID", TimelineNumber - 1);
            parametersFinished.Add("AgeID", AgeNumber);
            parametersFinished.Add("BattleID", TimelineState.MaxBattle + 1);
            _sender.CustomEvent("timeline_finished", parametersFinished);

            _sender.SetUserData("difficulty", $"{(_difficulty.GetDifficultyValue(TimelineNumber) - 1) * 100}%");
        }
#if UNITY_IOS
        private void IDFAStatus(ATTrackingStatusBinding.AuthorizationTrackingStatus currentStatus)
        {
            string idfaStatus;

            switch (currentStatus)
            {
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED:
                    idfaStatus = "NotDetermined";
                    break;
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED:
                    idfaStatus = "Restricted";
                    break;
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED:
                    idfaStatus = "Denied";
                    break;
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED:
                    idfaStatus = "Authorized";
                    break;
                default:
                    idfaStatus = "Unknown";
                    break;
            }

            var parameters = new DTDCustomEventParameters();
            parameters.Add("IDFAStatus", idfaStatus);
            _sender.CustomEvent("idfa_status", parameters);
            Debug.Log("IDFA Status: " + idfaStatus);

            //TrackPushNotifStatus(_pushStatusTemp);
        }
        public void SetPushNotifStatus(string status) => _pushStatusTemp = status;
        public void TrackPushNotifStatus(string status)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("PUSHStatus", status);

            _sender.CustomEvent("push_status", parameters);
            Debug.Log("Push Status BB " + status);
        }
#endif

        private void OnCompletedBattleChanged()
        {
            if (BattleStatistics.BattlesCompleted == 1 && TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                _sender.CustomEvent("first_build_success");
                _logger.Log("first_build_success", DebugStatus.Info);
            }

            else if (BattleStatistics.BattlesCompleted == 1 && !TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                _sender.CustomEvent("first_build_failed");
                _logger.Log("first_build_failed", DebugStatus.Info);
            }

            if (BattleStatistics.BattlesCompleted == 1 && TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                var paramSuccess = new DTDCustomEventParameters();
                paramSuccess.Add("value", "success");
                _sender.CustomEvent("first_warrior_spawn", paramSuccess);
            }

            else if (BattleStatistics.BattlesCompleted == 1 && !TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                var paramSuccess = new DTDCustomEventParameters();
                paramSuccess.Add("value", "failed");
                _sender.CustomEvent("first_warrior_spawn", paramSuccess);
            }
            if (BattleStatistics.BattlesCompleted == 1)
            {
                var parametersOpened = new DTDCustomEventParameters();
                parametersOpened.Add("TimelineID", TimelineNumber);
                parametersOpened.Add("AgeID", AgeNumber);
                parametersOpened.Add("BattleID", TimelineState.MaxBattle + 1);
                _sender.CustomEvent("timeline_opened", parametersOpened);
            }
        }

        private void TrackAdImpression(string adUnitID, MaxSdkBase.AdInfo adInfo, Placement placementType)
        {
            double revenue = adInfo.Revenue;
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode;
            string networkName = adInfo.NetworkName;
            string adUnitIdentifier = adUnitID;
            string placement = placementType.ToString();
            string networkPlacement = adInfo.NetworkPlacement;

            _sender.AdImpression(networkName, revenue, placement, adUnitIdentifier);
            _logger.Log("Log Data - AdImpression placement  " + placementType.ToString());
        }
        private void SendCustomAdImpression(AdType adType, Placement placement, MaxSdkBase.AdInfo info, int adsCount)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("TimelineID", (TimelineState.TimelineId + 1).ToString());
            parameters.Add("AgeID", (TimelineState.AgeId + 1).ToString());
            parameters.Add("BattleID", (TimelineState.MaxBattle + 1).ToString());
            parameters.Add("Level", TimelineState.Level.ToString());
            parameters.Add("Revenue", info.Revenue);
            parameters.Add("Network", info.NetworkName);
            parameters.Add("Placemment", placement.ToString());
            //parameters.Add("adCount", adsCount.ToString());
            //parameters.Add("type", adType.ToString());

            if (TimelineState.TimelineId < Constants.FeatureThresholds.DUNGEONS_TIMELINE_THRESHOLD && Dungeons.Dungeons.ToList().Count == 0)
            {
                parameters.Add("DungeonTimeline", 0);
                parameters.Add("DungeonLevel", 0);
                parameters.Add("DungeonBattle", 0);
            }
            else
            {
                int levelPerStage = 10;
                var ratsRush = Dungeons.Dungeons.ToList()[0];
                var stage = (ratsRush.Level - 1) / levelPerStage + 1;
                int subLevel = (ratsRush.Level - 1) % levelPerStage + 1;

                parameters.Add("DungeonTimeline", stage);
                parameters.Add("DungeonBattle", subLevel);
                parameters.Add("DungeonLevel", ratsRush.Level);
            }

            _sender.CustomEvent("custom_ad_impression", parameters);
        }

        private void TrackProfitIap(Product product)
        {
            if (product.definition.id == "profit1")
            {
                _sender.SetUserData("X2_profit_active(iAP)", "active");
            }
            ;
            if (product.definition.id == "profit2")
            {
                _sender.SetUserData("X3_profit_active(iAP)", "active");
            }
            ;
        }
        private void TrackFreeGemsPackCount(int id, int left) =>
            _sender.SetUserData($"free_gems_pack_balance_{id}", left.ToString());

        private void TrackAdsGemsPackCount(int id, int left) =>
           _sender.SetUserData($"ads_gems_pack_balance_{id}", left.ToString());
        private void SetUserDataEvolution()
        {
            _sender.SetUserData("evolution", (TimelineState.AgeId + 1).ToString());
        }
        private void SetUserDataCountAds(int count)
        {
            _sender.SetUserData("viewed_ads", count.ToString());
        }
        private void SetUserDataBattle()
        {
            _sender.SetUserData("battle", (TimelineState.MaxBattle + 1).ToString());
        }
        private void SetUserDataTimeline()
        {
            _sender.SetUserData("timeline", (TimelineState.TimelineId + 1).ToString());
        }

    }
}
