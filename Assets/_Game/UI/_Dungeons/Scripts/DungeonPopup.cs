using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;


namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonPopup : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;

        [SerializeField, Required] private TMP_Text _nameLabel;
        [SerializeField, Required] private TMP_Text _difficulty;

        [SerializeField, Required] private Button _previousDifficultyBtn;
        [SerializeField, Required] private Button _nextDifficultyBtn;

        [SerializeField, Required] private Image _rewardImage;
        [SerializeField, Required] private TMP_Text _rewardAmountLabel;

        [SerializeField, Required] private Button[] _exitButtons;

        [SerializeField, Required] private ThemedButton _enterButton;
        [SerializeField, Required] private TMP_Text _buttonLabel;

        //[SerializeField] private AmountListView _amountListView;

        [SerializeField, Required] private AmountView _amountView;

        [SerializeField, Required] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private DungeonPopupPresenter _presenter;

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            DungeonPopupPresenter presenter)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = presenter;

            _audioService = audioService;
            
            Init();
        }

        private void Init()
        {
            _presenter.StateChanged += OnStateChanged;
            OnStateChanged();

            foreach (var button in _exitButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }

            _nameLabel.text = _presenter.GetDungeonName();

            _previousDifficultyBtn.onClick.AddListener(OnPreviousClicked);
            _nextDifficultyBtn.onClick.AddListener(OnNextClicked);
            _enterButton.onClick.AddListener(OnEnter);

            _rewardImage.sprite = _presenter.GetRewardIcon();
        }

        private void OnStateChanged()
        {
            _difficulty.text = _presenter.GetDifficulty();

            _amountView.SetIcon(_presenter.GetIcon());
            _amountView.SetAmount(_presenter.GetBalance());
            _rewardAmountLabel.text = _presenter.GetRewardAmount();

            bool isPreviousBtnActive = _presenter.CanMovPrevious();
            _previousDifficultyBtn.interactable = isPreviousBtnActive;
            _previousDifficultyBtn.gameObject.SetActive(isPreviousBtnActive);

            bool isNextBtnActive = _presenter.CanMoveNext();
            _nextDifficultyBtn.interactable = isNextBtnActive;
            _nextDifficultyBtn.gameObject.SetActive(isNextBtnActive);

            var canEnter = _presenter.CanEnter();
            _enterButton.SetInteractable(canEnter);
            _buttonLabel.text = "Enter";
        }

        private void OnEnter() => 
            _presenter.OnEnter();

        public void Dispose()
        {
            foreach (var button in _exitButtons)
            {
                button.onClick.RemoveListener(OnCancelled);
            }

            _previousDifficultyBtn.onClick.RemoveListener(OnPreviousClicked);
            _nextDifficultyBtn.onClick.RemoveListener(OnNextClicked);

            _enterButton.onClick.RemoveListener(OnEnter);

            _presenter.StateChanged -= OnStateChanged;
        }

        public async UniTask<bool> AwaitForDecision()
        {
            _canvas.enabled = true;
            _animation.PlayShow();

            _taskCompletion = new UniTaskCompletionSource<bool>();
            bool result = await _taskCompletion.Task;
            return result;
        }
        
        private void OnNextClicked()
        {
            if (_presenter.CanMoveNext())
            {
                _presenter.MoveNext();
            }

            _audioService.PlayButtonSound();
        }

        private void OnPreviousClicked()
        {
            if (_presenter.CanMovPrevious())
            {
                _presenter.MovePrevious();
            }

            _audioService.PlayButtonSound();
        }

        [Button]
        private void OnCancelled()
        {
            DisableButtons();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }

        private void DisableButtons()
        {
            foreach (var button in _exitButtons)
            {
                button.interactable = false;
            }

            _nextDifficultyBtn.interactable = false;
            _previousDifficultyBtn.interactable = false;
            _enterButton.interactable = false;
        }

        private void OnHideComplete()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(false);
        }

        public void SetActive(bool isActive) =>
            gameObject.SetActive(isActive);
    }
}