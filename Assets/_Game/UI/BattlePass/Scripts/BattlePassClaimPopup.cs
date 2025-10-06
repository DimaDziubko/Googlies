using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassClaimPopup : MonoBehaviour
    {
        [SerializeField, Required] private UIDocument _uiDocument;
        
        [SerializeField, Required] private VisualElementListView _unclaimedRewardListView;
        
        private PopupAppearanceAnimationToolkit _animation;
        
        private BattlePassClaimPopupPresenter _presenter;
        
        private IAudioService _audioService;
        private IMyLogger _logger;

        private Button _exitButton1;
        private Button _claimBtn;
        
        private VisualElement _popupContainer;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private UnclaimedRewardListViewPresenter _unclaimedRewardListViewPresenter;

        public void Construct(
            BattlePassClaimPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;
            _presenter = presenter;
            _audioService = audioService;
            InitComponents();
            InitValues();
        }
        
        public async UniTask<bool> AwaitForExit()
        {
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void InitComponents()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            
            _exitButton1 = root.Q<Button>("BP-exit-btn-bg");
            _claimBtn = root.Q<Button>("Common-purchase-btn");
            if(_claimBtn != null)
                _claimBtn.text = "Claim";
            
            _popupContainer = root.Q<VisualElement>("BP-popup-container");
            
            var rewardContainer = root.Q<VisualElement>("BP-reward-container");

            _unclaimedRewardListView.Initialize(rewardContainer);
            
            _unclaimedRewardListViewPresenter =
                new UnclaimedRewardListViewPresenter(_unclaimedRewardListView, _presenter.GetRewards());
            
            _unclaimedRewardListViewPresenter.Initialize();
        }

        private void InitValues()
        {
            InitAppearanceAnimation();
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

        public void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
        [Button]
        private void OnExitButtonClicked()
        {
            PlayButtonSound();
            
            _exitButton1.SetEnabled(false);

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
            
            _presenter.Claim();
        }

        private void OnHideComplete()
        {
            _taskCompletion.TrySetResult(true);
        }

        public void Dispose()
        {
            _unclaimedRewardListViewPresenter.Dispose();
            Unsubscribe();
            _animation?.Dispose();
        }
        
        private void Subscribe()
        {
            if (_exitButton1 != null)
                _exitButton1.clicked += OnExitButtonClicked;
            if (_claimBtn != null)
                _claimBtn.clicked += OnExitButtonClicked;
        }
        
        private void Unsubscribe()
        {
            if (_exitButton1 != null)
                _exitButton1.clicked -= OnExitButtonClicked;
            if (_claimBtn != null)
                _claimBtn.clicked -= OnExitButtonClicked;
        }
    }
}