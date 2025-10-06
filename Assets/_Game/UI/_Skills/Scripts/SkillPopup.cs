using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;
using static Balancy.UnnyLogger;

namespace _Game.UI._Skills.Scripts
{
    public class SkillPopup : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private TMP_Text _skillNameLabel;
        [SerializeField, Required] private TextMeshProUGUI _skillDescriptionLabel;
        [SerializeField, Required] private SkillItemView _skillItemView;

        [SerializeField, Required] private ThemedButton _equipButton;
        [SerializeField, Required] private ThemedButton _removeButton;
        [SerializeField, Required] private ThemedButton _upgradeButton;
        [SerializeField, Required] private ThemedButton _ascendButton;

        [SerializeField, Required] private GameObject _equipButtonContainer;
        [SerializeField, Required] private GameObject _removeButtonContainer;
        [SerializeField, Required] private GameObject _upgradeButtonContainer;
        [SerializeField, Required] private GameObject _ascendButtonContainer;

        [SerializeField, Required] private GameObject _hint;
        [SerializeField, Required] private Button _hintButton;

        [SerializeField, Required] private Button[] _cancelButtons;

        [SerializeField, Required] private PopupAppearanceAnimation _animation;

        [SerializeField, Required] private AscendPopup _ascendPopup;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private IIconConfigRepository _config;
        private IMyLogger _logger;
        private IUserContainer _userContainer;
        private BoostContainer _boosts;

        private SkillPopupPresenter _skillPopupPresenter;
        private SkillItemViewPresenter _viewPresenter;

        private AscendPopupPresenter _ascendPopupPresenter;

        private readonly Dictionary<Boost, BoostInfoItem> _passiveBoostInfo = new();

        [SerializeField] private PassiveBoostInfoListView _passiveBoostInfoListView;

        [SerializeField] private BoostInfoAnimationListView _boostInfoAnimationListView;

        private readonly List<BoostInfoAnimationPresenter> _presenters = new();

        public void Construct(
            SkillPopupPresenter skillPopupPresenter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _skillPopupPresenter = skillPopupPresenter;
            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            _userContainer = userContainer;

            Initialize();
        }

        private void Initialize()
        {
            Unsubscribe();
            Subscribe();

            InitializeAnimation();

            _skillPopupPresenter.Initialize();

            _skillNameLabel.text = _skillPopupPresenter.GetName();

            _viewPresenter ??= new SkillItemViewPresenter(_skillPopupPresenter.Model, _skillItemView, _userContainer, _logger);
            _viewPresenter.InitializeNotClickable();

            _ascendPopupPresenter ??= new AscendPopupPresenter(_skillPopupPresenter.Model, _ascendPopup, _logger, _audioService, _config);
            _ascendPopupPresenter.Initialize();

            _hint.SetActive(false);

            OnStateChanged();
        }

        private void InitializeAnimation()
        {
            IEnumerable<BoostModel> boosts = _boosts.GetBoostModels(BoostSource.Total);

            foreach (BoostModel boost in boosts)
            {
                BoostInfoAnimationView view = _boostInfoAnimationListView.SpawnElement();
                BoostInfoAnimationPresenter presenter = new BoostInfoAnimationPresenter(boost, view, _config);
                presenter.Initialize();
                _presenters.Add(presenter);
            }
        }


        private void UpdateBoostInfo()
        {
            _passiveBoostInfoListView.gameObject.SetActive(_skillPopupPresenter.HasPassiveBoosts());

            for (int i = 0; i < _skillPopupPresenter.GetBoosts().Count(); i++)
            {
                var boost = _skillPopupPresenter.GetBoost(i);

                if (!_passiveBoostInfo.TryGetValue(boost, out var view))
                {
                    view = _passiveBoostInfoListView.SpawnElement();
                    _passiveBoostInfo.Add(boost, view);
                }

                float currentPassiveValue = _skillPopupPresenter.GetCurrentPassiveFor(i);
                float nextPassiveValue = _skillPopupPresenter.GetNextPassiveFor(i);

                view.SetIcon(_config.ForBoostIcon(boost.Type));
                view.SetName(boost.Name);
                //view.SetCurrentValue(currentPassiveValue.ToCompactFormat(0, true));
                //view.SetNextValue(nextPassiveValue.ToCompactFormat(0, true));
                //view.SetNextValueActive(!_skillPopupPresenter.IsAscendAvailable);

                view.SetValue($"<color=white>{currentPassiveValue.ToCompactFormat()}</color>" +
    $"<color=green> > {nextPassiveValue.ToCompactFormat()}</color>");

                _skillDescriptionLabel.text = _skillPopupPresenter.GetDescription();
            }
        }

        private void Subscribe()
        {
            _skillPopupPresenter.StateChanged += OnStateChanged;

            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            _equipButton.onClick.AddListener(OnEquipButtonClicked);
            _removeButton.onClick.AddListener(OnRemoveButtonClicked);
            _ascendButton.onClick.AddListener(OnAscendButtonClicked);
            _hintButton.onClick.AddListener(OnHintButtonClicked);

            foreach (var button in _cancelButtons)
                button.onClick.AddListener(OnCancelled);
        }

        private void OnEquipButtonClicked()
        {
            PlayButtonSound();
            _skillPopupPresenter.RequestEquip();
        }

        private void OnRemoveButtonClicked()
        {
            PlayButtonSound();
            _skillPopupPresenter.RequestUnEquip();
        }

        private void OnAscendButtonClicked()
        {
            PlayButtonSound();
            _ascendPopupPresenter.Show();
        }

        private void Unsubscribe()
        {
            _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
            _equipButton.onClick.RemoveListener(OnEquipButtonClicked);
            _removeButton.onClick.RemoveListener(OnRemoveButtonClicked);
            _ascendButton.onClick.RemoveListener(OnAscendButtonClicked);
            _hintButton.onClick.RemoveListener(OnHintButtonClicked);

            _skillPopupPresenter.StateChanged -= OnStateChanged;

            foreach (var button in _cancelButtons)
                button.onClick.RemoveAllListeners();
        }

        private void OnHintButtonClicked()
        {
            PlayButtonSound();
            _hint.SetActive(!_hint.activeInHierarchy);
        }

        private void OnStateChanged()
        {
            _upgradeButton.SetInteractable(_skillPopupPresenter.IsReadyForUpgrade);
            _skillDescriptionLabel.text = _skillPopupPresenter.GetDescription();

            UpdateBoostInfo();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            _equipButtonContainer.SetActive(!_skillPopupPresenter.IsEquipped);
            _removeButtonContainer.SetActive(_skillPopupPresenter.IsEquipped);
            _upgradeButtonContainer.SetActive(!_skillPopupPresenter.IsAscendAvailable);
            _ascendButtonContainer.SetActive(_skillPopupPresenter.IsAscendAvailable);

            _equipButton.SetInteractable(true);
            _removeButton.SetInteractable(true);

            _upgradeButton.SetInteractable(_skillPopupPresenter.IsReadyForUpgrade);
            _ascendButton.SetInteractable(true);
        }

        public async UniTask<bool> ShowDetailsAndAwaitForExit()
        {
            _animation.PlayShow(OnShowComplete);

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void OnUpgradeButtonClicked()
        {
            PlayUpgradeSound();
            _skillPopupPresenter.Upgrade();
        }

        private void OnCancelled()
        {
            DisableButtons();
            PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }

        private void DisableButtons()
        {
            foreach (var button in _cancelButtons)
            {
                button.interactable = false;
            }

            _upgradeButton.interactable = false;
        }

        private void OnHideComplete()
        {
            _canvas.enabled = false;
            Cleanup();
            _taskCompletion.TrySetResult(true);
        }

        private void Cleanup()
        {
            _skillPopupPresenter.Dispose();
            _viewPresenter.Dispose();
            _ascendPopupPresenter.Dispose();

            _passiveBoostInfo.Clear();

            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }
            _presenters.Clear();


            Unsubscribe();
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
        private void PlayUpgradeSound() => _audioService.PlayUpgradeSound();
    }
}