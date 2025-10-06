using System;
using _Game.Common;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core.Ads;
using _Game.Core.Services.Audio;
using _Game.Gameplay._FoodBoost.Scripts;
using _Game.Gameplay.Common;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.UI._Hud._FoodBoostView
{
    public class FoodBoostPresenter : IDisposable, IStartGameListener, IStopGameListener
    {
        private readonly FoodBoostService _foodBoostService;
        private readonly IAudioService _audioService;
        private readonly BattleHud _hud;
        private readonly IFoodContainer _foodContainer;
        private readonly IAdsService _adsService;
        private readonly UpgradeItemContainer _upgradeItemContainer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private FoodBoostBtn View => _hud.FoodBoostBtn;
        private FoodBoostBtnModel Model => _foodBoostService.Model;

        private bool _isSpent;

        public FoodBoostPresenter(
            UpgradeItemContainer upgradeItemContainer,
            FoodBoostService foodBoostService,
            IAudioService audioService,
            BattleHud hud,
            IFoodContainer foodContainer,
            IAdsService adsService,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _foodBoostService = foodBoostService;
            _audioService = audioService;
            _hud = hud;
            _foodContainer = foodContainer;
            _adsService = adsService;
            _upgradeItemContainer = upgradeItemContainer;
            _featureUnlockSystem = featureUnlockSystem;
            Init();
        }

        private void Init() => View.SetActive(false);

        private void UpdateView()
        {
            View.SetIcon(Model.Icon);
            View.SetValue(CalculateAmount().ToString());
            CheckActive();
            UpdateButtonState();
        }

        private void CheckActive()
        {
            View.SetActive(
                !_isSpent &&
                Model.IsAvailableBoost && 
                _featureUnlockSystem.IsFeatureUnlocked(Feature.FoodBoost));
        }

        void IStartGameListener.OnStartBattle()
        {
            UpdateView();
            Subscribe();
        }

        void IStopGameListener.OnStopBattle()
        {
            View.SetActive(false);
            _isSpent = false;
            Unsubscribe();
        }

        void IDisposable.Dispose() => Unsubscribe();

        private void Subscribe()
        {
            View.ButtonClicked += OnClicked;
            _adsService.RewardedVideoLoaded += OnVideoLoaded;
        }

        private void Unsubscribe()
        {
            View.ButtonClicked -= OnClicked;
            _adsService.RewardedVideoLoaded -= OnVideoLoaded;
        }

        private void OnVideoLoaded() => UpdateButtonState();

        private void UpdateButtonState()
        {
            bool isInteractable = _adsService.IsAdReady(AdType.Rewarded);
            View.SetInteractable(isInteractable);
        }

        private void OnClicked()
        {
            _audioService.PlayButtonSound();
            _adsService.ShowRewardedVideo(OnRewardedComplete, Placement.Food);
            _isSpent = true;
            CheckActive();
            UpdateButtonState();
        }

        private void OnRewardedComplete()
        {
            Model.SpendFoodBoost();
            _foodContainer.Add(CalculateAmount());
        }

        private int CalculateAmount() => 
            (int)(Model.Coefficient * _upgradeItemContainer.GetItem(UpgradeItemType.FoodProduction).Value);
    }
}