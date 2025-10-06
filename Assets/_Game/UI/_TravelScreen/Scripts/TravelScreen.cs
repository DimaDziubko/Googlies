using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services._Camera;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.Utils.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;

namespace _Game.UI._TravelScreen.Scripts
{
    public class TravelScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField, Required] private TMP_Text _travelInfo;
        [SerializeField, Required] private ThemedButton _travelButton;
        [SerializeField, Required] private TMP_Text _travelConditionHint;
        [SerializeField, Required] private TMP_Text _travelText;
        [SerializeField, Required] private AmountView _rewardView;
        
        private ITravelScreenPresenter _presenter;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IConfigRepository _config;

        public void Construct(
            IWorldCameraService cameraService,
            ITravelScreenPresenter presenter,
            IConfigRepository config,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _config = config;
            _featureUnlockSystem = featureUnlockSystem;
            _presenter = presenter;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _canvas.enabled = false;
        }

        public void Show()
        {
            Unsubscribe();
            Subscribe();

            UpdateView();

            _presenter.OnScreenOpen();
            _canvas.enabled = true;
        }

        private void UpdateView()
        {
            _travelButton.SetInteractable(_presenter.CanTravel);
            _travelConditionHint.enabled = !_presenter.CanTravel;
            _travelConditionHint.text = _presenter.GetTravelConditionHint();
            _travelInfo.text = _presenter.GetTravelInfo();
            _travelText.text = _presenter.GetTravelText();
            
            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills))
            {
                _rewardView.SetActive(true);
                _rewardView.SetIcon(_config.IconConfigRepository.GetCurrencyIconFor(CurrencyType.SkillPotion));
                _rewardView.SetAmount(_config.SkillConfigRepository.RewardPerEvolve.Amount.ToCompactFormat());
            }
            else
            {
                _rewardView.SetActive(false);
            }
        }

        private void Subscribe()
        {
            _travelButton.onClick.AddListener(_presenter.OnTravelButtonClicked);
            _presenter.StateChanged += OnStateChanged;
        }

        private void OnStateChanged() => UpdateView();


        public void Hide()
        {
            Unsubscribe();
            _canvas.enabled = false;
            _presenter.OnScreenClosed();
        }

        public void Dispose()
        {
            Unsubscribe();
            _presenter.OnScreenDisposed();
        }
        
        private void Unsubscribe()
        {
            _travelButton.onClick.RemoveListener(_presenter.OnTravelButtonClicked);
            _presenter.StateChanged -= OnStateChanged;
        }

        [Button] //Debug
        private void ForceTravel() => _presenter.OnTravelButtonClicked();

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnScreenActiveChanged(isActive);
        }
    }
}