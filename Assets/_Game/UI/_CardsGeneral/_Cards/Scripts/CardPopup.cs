using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._Skills.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardPopup : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private TMP_Text _cardNameLabel;
        [SerializeField, Required] private TMP_Text _cardTypeLabel;
        [SerializeField, Required] private TMP_Text _cardDescriptionLabel;
        [SerializeField, Required] private CardItemView _cardItemView;
        [SerializeField, Required] private ThemedButton _upgradeButton;
        [SerializeField, Required] private Button[] _cancelButtons;

        [SerializeField, Required] private PassiveBoostInfoListView _passiveBoostInfoListView;
        [SerializeField, Required] private BoostInfoAnimationListView _boostInfoAnimationListView;
        
        [SerializeField, Required] private PopupAppearanceAnimation _animation;

          private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;
        private IIconConfigRepository _config;
        private BoostContainer _boosts;
        private IMyLogger _logger;

        private CardPopupPresenter _cardPopupPresenter;
        private CardItemPresenter _presenter;

        private readonly Dictionary<Boost, PassiveBoostInfoView> _cardBoostInfo = new();

        private readonly List<BoostInfoAnimationPresenter> _presenters = new();

        public void Construct(
            CardPopupPresenter cardPopupPresenter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IMyLogger logger)
        {
            _cardPopupPresenter = cardPopupPresenter;
            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            
            Initialize();
        }

        private void Initialize()
        {
            Unsubscribe();
            Subscribe();

            InitializeAnimation();
            
            _cardPopupPresenter.Initialize();
            
            _cardNameLabel.text = _cardPopupPresenter.GetName();
            _cardTypeLabel.text = _cardPopupPresenter.GetTypeName();
            _cardTypeLabel.color = _cardPopupPresenter.GetTypeColor();
            _cardDescriptionLabel.text = _cardPopupPresenter.GetDescription();
            _presenter ??= new CardItemPresenter(_cardPopupPresenter.Model, _cardItemView, _logger);
            _presenter.InitializeNotClickable();
            
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
            foreach (Boost boost in _cardPopupPresenter.GetBoosts())
            {
                int level = _cardPopupPresenter.Level;

                if (!_cardBoostInfo.TryGetValue(boost, out var view))
                {
                    view = _passiveBoostInfoListView.SpawnElement();
                    _cardBoostInfo.Add(boost, view);
                }

                view.SetIcon(_config.ForBoostIcon(boost.Type));
                view.SetName(boost.Name);
                view.SetCurrentValue(boost.Exponential.GetValue(level).ToCompactFormat());
                view.SetNextValue(boost.Exponential.GetValue(level + 1).ToCompactFormat());
            }
        }

        private void Unsubscribe()
        {
            _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
            _cardPopupPresenter.StateChanged -= OnStateChanged;
            
            foreach (var button in _cancelButtons) 
                button.onClick.RemoveAllListeners();
        }

        private void Subscribe()
        {
            _cardPopupPresenter.StateChanged += OnStateChanged;
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            
            foreach (var button in _cancelButtons) 
                button.onClick.AddListener(OnCancelled);
        }

        private void OnStateChanged()
        {
            _upgradeButton.SetInteractable(_cardPopupPresenter.IsReadyForUpgrade);
            UpdateBoostInfo();
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
            _cardPopupPresenter.Upgrade();
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
            _cardPopupPresenter.Dispose();
            _presenter.Dispose();
            _cardBoostInfo.Clear();

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