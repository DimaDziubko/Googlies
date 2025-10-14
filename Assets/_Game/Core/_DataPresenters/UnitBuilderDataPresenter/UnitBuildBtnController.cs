using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Gameplay._UnitBuilder.Scripts;
using UnityEngine;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public class UnitBuildBtnController
    {
        private readonly Color _unitIconAffordableColor = new(1f, 1f, 1f, 1f);
        private readonly Color _unitIconExpensiveColor = new(1f, 1f, 1f, 0.3f);

        private UnitBuilderBtnModel _model;
        private UnitBuildButton _view;
        private readonly IUnitBuilder _builder;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        private bool _isAvailableByFood;
        private bool _isPaused;

        public UnitBuildBtnController(
            UnitBuilderBtnModel model,
            UnitBuildButton view,
            IUnitBuilder builder,
            IAudioService audioService,
            IMyLogger logger)
        {
            _model = model;
            _view = view;
            _builder = builder;
            _audioService = audioService;
            _logger = logger;
        }

        public bool IsButtonReady => _isAvailableByFood;

        public void Initialize()
        {
            _logger.Log($"Icon is NULL {_model.Icon == null}, VIEW IS NULL {_view == null}");

            _view.SetUnitIcon(_model.Icon);
            _view.SetUnitTypeIconHolder(_model.UnitAttackSprite);
            _view.SetActive(_model.IsUnlocked);
            _view.SetFoodIcon(_model.FoodIcon);
            _view.Clicked += OnClicked;
        }

        public void Dispose() => _view.Clicked -= OnClicked;

        private void OnClicked()
        {
            _builder.Build(_model.Type, _model.FoodPrice);
            _audioService.PlayButtonSound();
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
            UpdateInteractableState();
        }

        public void HideBtn() => _view.SetActive(false);

        public void OnFoodBalanceChanged(int value)
        {
            _isAvailableByFood = _model.FoodPrice <= value;

            string price = _isAvailableByFood
                ? $"<color=white>{_model.FoodPrice}</color>"
                : $"<color=red>{_model.FoodPrice}</color>";

            _view.SetPrice(price);

            Color imageColor = _isAvailableByFood
                ? _unitIconAffordableColor
                : _unitIconExpensiveColor;

            _view.SetUnitIconColor(imageColor);

            UpdateInteractableState();
        }

        private void UpdateInteractableState()
        {
            bool canInteract = !_isPaused && _isAvailableByFood;

            _view.SetInteractable(canInteract);
        }
    }
}