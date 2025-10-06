using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.UI.BattlePass.Scripts;
using _Game.Utils.MyUILibrary;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPopup : MonoBehaviour
    {
        [SerializeField, Required] private UIDocument _document;
        [SerializeField, Required] private VisualElementListView _itemsListView;
        
        private WarriorsFundPopupPresenter _presenter;
        private IAudioService _audioService;
        private IMyLogger _logger;
        
        [SerializeField, Required] private WarriorsFundPurchasePopup _warriorsFundPurchasePopup;
        
        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private Button _exitButton1;
        private Button _exitButton2;
        private Button _activateBtn;
        private VisualElement _popupContainer;
        private VisualElement _wfRoadContainer;
        
        private VerticalProgressBar _wfRoadProgressBar;
        
        private VisualElement _totalRewardIcon;
        private Label _totalRewardLabel;

        private PopupAppearanceAnimationToolkit _animation;

        private WarriorsFundPointGroupListPresenter _wfPointGroupListPresenter;

        public void Construct(
            WarriorsFundPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            SetActive(false);
            _warriorsFundPurchasePopup.SetActive(false);
            
            _warriorsFundPurchasePopup.Construct(presenter.GetPurchasePresenter(), audioService, logger);
            
            _logger = logger;
            _audioService = audioService;
            _presenter = presenter;
            InitComponents();
            InitValues();
            
            SetActive(true);
        }

        private void InitComponents()
        {
            _presenter.Initialize();
            
            VisualElement root = _document.rootVisualElement;
            
            if (root == null)
            {
                _logger.Log("UIDocument rootVisualElement is null", DebugStatus.Fault);
                return;
            }
   
            _exitButton1 = root.Q<Button>("Exit-btn-bg");
            _exitButton2 = root.Q<Button>("Exit-btn");
            _activateBtn = root.Q<Button>("Activate-btn");
   
            _popupContainer = root.Q<VisualElement>("Popup-container");
            
            _wfRoadContainer = root.Q<VisualElement>("road-container");

            _wfRoadProgressBar = root.Q<VerticalProgressBar>("Road-progress-bar");
            
            _totalRewardIcon = root.Q<VisualElement>("Total-reward-icon");
            _totalRewardLabel = root.Q<Label>("Total-reward-label");
            
            _itemsListView.Initialize(_wfRoadContainer);
            _wfPointGroupListPresenter = new WarriorsFundPointGroupListPresenter(_presenter.GetPoints(), _itemsListView, _logger);
            _wfPointGroupListPresenter.Initialize();
            
            _warriorsFundPurchasePopup.Initialize();
            _warriorsFundPurchasePopup.SetActive(false);
        }

        private void InitValues()
        {
            _activateBtn.SetEnabled(true);
            InitAppearanceAnimation();
            CheckActivateBtnState();
            InitTotalReward();
            OnRoadProgressChanged();
        }

        private void InitTotalReward()
        {
            if(_totalRewardIcon != null)
                _totalRewardIcon.style.backgroundImage = new StyleBackground(_presenter.GetTotalRewardIcon());
            if (_totalRewardLabel != null)
                _totalRewardLabel.text = _presenter.GetTotalRewardAmount();
        }

        private void CheckActivateBtnState()
        {
            if (_presenter.IsPurchased())
            {
                _activateBtn.style.display = DisplayStyle.None;
                return;
            }
            
            _activateBtn.style.display = DisplayStyle.Flex;
        }

        private void InitAppearanceAnimation()
        {
            if (_popupContainer != null)
            {
                _animation = new PopupAppearanceAnimationToolkit(_popupContainer);
                _animation.Show(OnShowComplete);
            }
            else
                OnShowComplete();
        }

        private void OnShowComplete() => Subscribe();

        private void Subscribe()
        {
            _logger.Log("WF SUBSCRIBE", DebugStatus.Info);
            
            if (_exitButton1 != null)
                _exitButton1.clicked += OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked += OnExitButtonClicked;
            if (_activateBtn != null)
                _activateBtn.clicked += OnWfActivateBtnClicked;
            
            _presenter.RoadProgressChanged += OnRoadProgressChanged;
            _presenter.Purchased += OnWarriorsFundPurchased;
        }

        private void OnRoadProgressChanged()
        {
            if (_wfRoadProgressBar != null)
            {
                _wfRoadProgressBar.Value = _presenter.GetRoadProgressValue();
                _logger.Log($"Road progress value {_presenter.GetRoadProgressValue()}", DebugStatus.Warning);
            } 
            else
            {
                _logger.Log("Road progress bar is null", DebugStatus.Warning);
            }
        }

        private void OnWfActivateBtnClicked()
        {
            _audioService.PlayButtonSound();
            _warriorsFundPurchasePopup.Show();
        }

        private void OnExitButtonClicked()
        {
            _exitButton1.SetEnabled(false);
            _exitButton2.SetEnabled(false);

            _audioService.PlayButtonSound();

            _logger.Log("EXIT CLICKED", DebugStatus.Info);

            if (_animation != null)
            {
                _logger.Log("EXIT ANIMATOR CLICKED", DebugStatus.Info);
                _animation.Hide(OnHideComplete);
            }
            else
            {
                _logger.Log("EXIT WITHOUT ANIMATOR CLICKED", DebugStatus.Info);
                OnHideComplete();
            }
        }

        private void OnHideComplete() => _taskCompletion.TrySetResult(true);

        public void Dispose()
        {
            if (_exitButton1 != null)
                _exitButton1.clicked -= OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked -= OnExitButtonClicked;
            if (_activateBtn != null)
                _activateBtn.clicked -= OnWfActivateBtnClicked;
            
            _presenter.RoadProgressChanged -= OnRoadProgressChanged;
            _presenter.Purchased -= OnWarriorsFundPurchased;
            
            _wfPointGroupListPresenter.Dispose();
            _presenter.Dispose();
        }

        private void OnWarriorsFundPurchased() => CheckActivateBtnState();
        
        public async UniTask<bool> AwaitForExit()
        {
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }
        
        private void SetActive(bool isActive)
        {
            if (isActive)
            {
                _document.rootVisualElement.style.display = DisplayStyle.Flex; 
                return;
            }
            
            _document.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}