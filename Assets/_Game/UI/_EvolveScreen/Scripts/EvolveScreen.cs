using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Age;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI._TimelineInfoPresenter;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveScreen : MonoBehaviour
    {
        public event UnityAction CloseClicked
        {
            add => _closeButton.onClick.AddListener(value);
            remove => _closeButton.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private TMP_Text _timelineLabel;
        [SerializeField, Required] private Image _currentAgeImage;
        [SerializeField, Required] private TMP_Text _currentAgeName;
        [SerializeField, Required] private Image _nextAgeImage;
        [SerializeField, Required] private TMP_Text _nextAgeName;
        [SerializeField, Required] private TransactionButton _evolveButton;
        [SerializeField, Required] private AmountView _rewardView;

        [Space]
        [SerializeField] private TimelineInfoScreen _testTimelineInfo;
        [Space]
        [SerializeField, Required] private Button _closeButton;

        private IEvolveScreenPresenter _presenter;
        private IMiniShopProvider _miniShopProvider;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IConfigRepository _config;

        public void Construct(
            IWorldCameraService cameraService,
            IEvolveScreenPresenter evolveScreenPresenter,
            IConfigRepository config,
            IFeatureUnlockSystem featureUnlockSystem,
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IMyLogger logger,
            IAdsService adsService,
            IAgeNavigator ageNavigator
            )
        {
            _featureUnlockSystem = featureUnlockSystem;
            _config = config;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = evolveScreenPresenter;
            _canvas.enabled = false;

            _testTimelineInfo.Construct(
                audioService,
                timelineInfoPresenter,
                logger,
                cameraService,
                adsService,
                ageNavigator
                );
        }

        public void Show()
        {
            Unsubscribe();
            Subscribe();
            _presenter.OnScreenOpen();
            OnStateChanged();

            _canvas.enabled = true;
        }

        private void OnStateChanged()
        {
            _timelineLabel.text = _presenter.GetTimelineNumber();

            _currentAgeImage.sprite = _presenter.GetCurrentAgeIcon();
            _nextAgeImage.sprite = _presenter.GetNextAgeIcon();

            _currentAgeName.text = _presenter.GetCurrentAgeName();
            _nextAgeName.text = _presenter.GetNextAgeName();

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

            OnButtonStateChanged();
        }

        private void Subscribe()
        {
            _evolveButton.ButtonClicked += _presenter.OnEvolveClicked;
            _evolveButton.InactiveClicked += _presenter.OnInactiveEvolveClicked;
            _presenter.StateChanged += OnStateChanged;
            _presenter.ButtonStateChanged += OnButtonStateChanged;
        }

        private void OnButtonStateChanged()
        {
            _evolveButton.SetPrice(_presenter.GetEvolutionPrice().ToCompactFormat());
            _evolveButton.SetCurrencyIcon(_presenter.GetCurrencyIcon());
            _evolveButton.SetMoneyPanelActive(_presenter.GetEvolutionPrice() > 0);
            _evolveButton.SetInteractable(_presenter.CanEvolve());
        }

        private void Unsubscribe()
        {
            _evolveButton.ButtonClicked -= _presenter.OnEvolveClicked;
            _evolveButton.InactiveClicked -= _presenter.OnInactiveEvolveClicked;
            _presenter.StateChanged -= OnStateChanged;
            _presenter.ButtonStateChanged -= OnButtonStateChanged;
        }

        public void Dispose()
        {
            Unsubscribe();
            _presenter.OnScreenDisposed();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnScreenActiveChanged(isActive);
        }
    }
}