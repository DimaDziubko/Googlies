using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class SummoningPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private ThemedButton _previousLevelButton;
        [SerializeField] private ThemedButton _nextLevelButton;
        [SerializeField] private Button[] _cancelButtons;
        
        [SerializeField] private CardSummoningView[] _cardSummoningViews;

        [SerializeField] private PopupAppearanceAnimation _animation;
        
        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private IUserContainer _userContainer;
        
        private SummoningPopupPresenter _presenter;

        private int _levelToShow;
        private int LevelToShow
        {
            get => _levelToShow;
            set
            {
                _levelToShow = value;
                UpdateViews();
            }
        }

        public void Construct(
            Camera cameraServiceUICameraOverlay, 
            IAudioService audioService,
            SummoningPopupPresenter popupPresenter)
        {
            _canvas.worldCamera = cameraServiceUICameraOverlay;
            _audioService = audioService;
            _presenter = popupPresenter;
        }

        public async UniTask<bool> ShowAndAwaitForExit()
        {
            Initialize();
            _canvas.enabled = true;
            _animation.PlayShow();
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void Initialize()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }

            LevelToShow = _presenter.Level;
            
            CheckButtonsState();

            _nextLevelButton.onClick.AddListener(OnNextClicked);
            _previousLevelButton.onClick.AddListener(OnPreviousClicked);
        }

        private void CheckButtonsState()
        {
            _nextLevelButton.SetInteractable(_presenter.CanMoveNext(LevelToShow));
            _previousLevelButton.SetInteractable(_presenter.CanMovePrevious(LevelToShow));
        }

        private void OnPreviousClicked()
        {
            LevelToShow--;
            CheckButtonsState();
            PlayButtonSound();
        }

        private void OnNextClicked()
        {
            LevelToShow++;
            CheckButtonsState();
            PlayButtonSound();
        }

        private void UpdateViews()
        {
            _levelLabel.text = $"Level {LevelToShow.ToString()}";
            
            foreach (var view in _cardSummoningViews)
            {
                view.SetColor(_presenter.GetColorFor(view.CardType));
                view.SetValue(_presenter.GetSummoningValueFor(view.CardType, LevelToShow));
                view.SetTypeName(view.CardType.ToString());
            }
        }

        public void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void OnCancelled()
        {
            PlayButtonSound();
            _animation.PlayHide(OnHideCompleted);
        }
        
        private void OnHideCompleted()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
    }
}
