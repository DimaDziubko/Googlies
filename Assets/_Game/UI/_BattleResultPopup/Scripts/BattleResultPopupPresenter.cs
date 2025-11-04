using System;
using System.Linq;
using _Game.Common;
using _Game.Core.Ads;
using _Game.Core.UserState._State;
using _Game.Gameplay.Common;
using Zenject;

namespace _Game.UI._BattleResultPopup.Scripts
{
    public class BattleResultPopupPresenter : IInitializable, IDisposable
    {
        public event Action StateChanged;
        public event Action RewardVideoComplete;

        private readonly IAdsService _adsService;

        private readonly TemporaryCurrencyBank _temporaryBank;

        public BattleResultPopupPresenter(
            TemporaryCurrencyBank temporaryBank,
            IAdsService adsService)
        {
            _adsService = adsService;
            _temporaryBank = temporaryBank;
        }

        void IInitializable.Initialize() =>
            _adsService.InterstitialVideoLoaded += OnInterstitialVideoLoaded;

        void IDisposable.Dispose() =>
            _adsService.InterstitialVideoLoaded -= OnInterstitialVideoLoaded;

        private void OnInterstitialVideoLoaded() =>
            StateChanged?.Invoke();

        public void TryToShowInterstitial()
        {
            if (_adsService.IsAdReady(AdType.Interstitial))
                _adsService.ShowInterstitialVideo(Placement.X2);
        }

        public bool IsAdsButtonInteractable =>
            _adsService.IsAdReady(AdType.Rewarded);

        public void OnDoubleCoinsClicked()
        {
            UnityEngine.Debug.Log("[BattleResultPopupPresenter] OnDoubleCoinsClicked");
            if (_adsService.IsAdReady(AdType.Rewarded))
            {
                _adsService.ShowRewardedVideo(OnVideoCompleted, Placement.X2);
            }
        }

        private void OnVideoCompleted()
        {
            _temporaryBank.FirstOrDefault(cell => cell.Type == CurrencyType.Coins)?.Add(
                _temporaryBank.FirstOrDefault(cell => cell.Type == CurrencyType.Coins)?.Amount ?? 0);
            //foreach (var cell in _temporaryBank)
            //{
            //    cell.Add(cell.Amount);
            //}

            UnityEngine.Debug.Log("[BattleResultPopupPresenter] OnVideoCompleted");
            RewardVideoComplete?.Invoke();
        }

        public TemporaryCurrencyBank GetAdditionalRewards() =>
            _temporaryBank;

    }
}