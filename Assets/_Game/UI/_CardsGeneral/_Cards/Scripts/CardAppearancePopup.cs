using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.UI._Shop.Scripts._DecorAndUtils;
using _Game.UI.Factory;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardAppearancePopup : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private CardView _singleView;
        [SerializeField, Required] private Button _exitButton;
        [SerializeField, Required] private Transform _container;
        
        [SerializeField] private int _appearanceDelay = 1000;
        [SerializeField] private int _collectionAppearanceDelay = 200;
        [SerializeField] private int _illuminationAnimationDelay = 200;

        [SerializeField, Required] private GridLayoutGroup _group;
        [SerializeField, Required] private DynamicGridLayout _dynamicGridLayout;

        private IAudioService _audioService;
        private IUIFactory _uiFactory;
        private readonly List<CardView> _cardViews = new();
        private CardAppearancePopupSettings _settings;

        private UniTaskCompletionSource<bool> _taskCompletion;

        [ShowInInspector] private readonly List<CardPresenter> _presenters = new();

        public void Construct(
            Camera cameraServiceUICameraOverlay,
            IAudioService audioService,
            IUIFactory uiFactory,
            CardAppearancePopupSettings settings)
        {
            //Overlay because of toolkit
            //_canvas.worldCamera = cameraServiceUICameraOverlay;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _settings = settings;

            _singleView.Hide();

            Init();

            _canvas.enabled = true;
        }

        private void Init()
        {
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnExitButtonClicked()
        {
            Cleanup();
            _taskCompletion.TrySetResult(true);
            PlayButtonSound();
        }

        public async UniTask<bool> ShowAnimationAndAwaitForExit(
            List<CardModel> cardModelsForAnimation)
        {
            _exitButton.interactable = false;
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();

            ApplySettings(cardModelsForAnimation.Count);
            await UniTask.Delay(_appearanceDelay);
            if (cardModelsForAnimation.Count == 1)
            {
                InitCard(cardModelsForAnimation[0], _singleView);
            }
            else
            {
                await SpawnCards(cardModelsForAnimation.Count);
                InitCards(cardModelsForAnimation);
            }

            await PlayCollectionAnimation(_presenters);

            _exitButton.interactable = true;
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void ApplySettings(int count)
        {
            var settings = _settings.GetSettingsForAmount(count);
            if (settings != null)
            {
                _group.constraintCount = settings.ColumnsCount;
                _dynamicGridLayout.SetColumns(settings.ColumnsCount);
                _dynamicGridLayout.SetSpacing(settings.Spacing);
                _dynamicGridLayout.AdjustCellSize();
            }
        }

        private async UniTask SpawnCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var view = _uiFactory.GetCardView(_container);
                view.Hide();
                view.Resize();
                _cardViews.Add(view);
            }
            
            await UniTask.Yield();
            
            foreach (var view in _cardViews)
            {
                view.Resize();
            }
        }

        private void InitCard(CardModel model, CardView view)
        {
            CardPresenter presenter = new CardPresenter(model, view);
            presenter.Initialize();
            _presenters.Add(presenter);
        }

        private void InitCards(List<CardModel> models)
        {
            for (int i = 0; i < models.Count && i < _cardViews.Count; i++)
            {
                InitCard(models[i], _cardViews[i]);
            }
        }

        private async UniTask PlayCollectionAnimation(List<CardPresenter> presenters)
        {
            foreach (var presenter in presenters)
            {
                await PlaySingleAnimation(presenter);
                await UniTask.Delay(_collectionAppearanceDelay);
            }
        }

        private async UniTask PlaySingleAnimation(CardPresenter presenter)
        {
            PlayAppearanceSound();

            if (presenter.NeedIlluminationAnimation)
            {
                presenter.PlayRippleAppearanceAnimation(PlayRippleSound);
                await UniTask.Delay(_illuminationAnimationDelay);
            }
            else
            {
                presenter.PlaySimpleAppearanceAnimation();
            }

            await UniTask.Yield();
        }

        private void PlayAppearanceSound() => _audioService.PlayCardAppearanceSfx();
        private void PlayRippleSound() => _audioService.PlayCardRippleSfx();
        private void PlayButtonSound() => _audioService.PlayButtonSound();
        
        private void Cleanup()
        {
            _exitButton.onClick.RemoveAllListeners();
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }
            foreach (var view in _cardViews)
            {
                view.Recycle();
            }
            
            _cardViews.Clear();
            _presenters.Clear();
        }
    }
}