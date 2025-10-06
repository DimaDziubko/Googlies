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
            if (_adsService.IsAdReady(AdType.Rewarded))
            {
                _adsService.ShowRewardedVideo(OnVideoCompleted, Placement.X2);
            }
        }

        private void OnVideoCompleted()
        {
            foreach (var cell in _temporaryBank)
            {
                cell.Add(cell.Amount);
            }
            
            RewardVideoComplete?.Invoke();
        }

        public TemporaryCurrencyBank GetAdditionalRewards() => 
            _temporaryBank;
        
    }
}