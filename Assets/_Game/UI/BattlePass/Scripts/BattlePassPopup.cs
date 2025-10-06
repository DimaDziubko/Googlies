using System;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Utils.Extensions;
using _Game.Utils.MyUILibrary;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassPopup : MonoBehaviour
    {
        private const string CONTROL_POINT_ACTIVE_STYLE_NAME = "active";
        
        [SerializeField, Required] private UIDocument _document;
        [SerializeField, Required] private VisualElementListView _listView;

        [FormerlySerializedAs("_leaderPassInfoPopup")] [SerializeField, Required] private BattlePassInfoPopup battlePassInfoPopup;
        [FormerlySerializedAs("_leaderPassPurchasePopup")] [SerializeField, Required] private BattlePassPurchasePopup battlePassPurchasePopup;
        
        private IAudioService _audioService;
        private IMyLogger _logger;

        [ShowInInspector, ReadOnly]
        private BattlePassPopupPresenter _presenter;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private Button _exitButton1, _exitButton2;
        private Button _bpActivateBtn;
        private Button _bpInfoBtn;

        private PopupAppearanceAnimationToolkit _animation;

        private VisualElement _popupContainer;
        private Label _timerLabel;
        private VisualElement _bpPointIcon;
        private Label _conditionLabel;
        private VisualElement _conditionTextContainer;
        private VisualElement _bpRoadContainer;

        private BattlePassPointGroupListPresenter _bpPointGroupListPresenter;

        private ProgressBar _bpPointsProgressBar;

        private VisualElement _controlPointContainer;
        private VisualElement _controlPointIconHolder;
        private Label _controlPointLabel;

        private VerticalProgressBar _bpRoadProgressBar;
        private ScrollView _scrollView;
        
        public void Construct(
            BattlePassPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            SetActive(false);
            battlePassPurchasePopup.SetActive(false);
            battlePassInfoPopup.SetActive(false);
            
            battlePassPurchasePopup.Construct(presenter.GetPurchasePresenter(), audioService, logger);
            
            _presenter = presenter;
            _logger = logger;
            _audioService = audioService;
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

            _exitButton1 = root.Q<Button>("BP-exit-btn-bg");
            _exitButton2 = root.Q<Button>("Exit-btn");
            _bpActivateBtn = root.Q<Button>("BP-activate-btn");
            _bpInfoBtn = root.Q<Button>("BP-info-btn");
            _timerLabel = root.Q<Label>("BP-timer-label");

            _popupContainer = root.Q<VisualElement>("BP-popup-container");
            _bpPointIcon = root.Q<VisualElement>("BP-point-icon");
            _conditionLabel = root.Q<Label>("BP-condition-text-label");
            _bpRoadContainer = root.Q<VisualElement>("BP-road-list-view");
            
            _controlPointContainer = root.Q<VisualElement>("BP-control-point-container-main");
            _controlPointIconHolder = root.Q<VisualElement>("BP-control-point-icon");
            _controlPointLabel = root.Q<Label>("BP-control-point-label");
            
            _bpPointsProgressBar = root.Q<ProgressBar>("BP-points-progress-bar");
            _bpRoadProgressBar = root.Q<VerticalProgressBar>("BP-road-progress-bar");
            
            _listView.Initialize(_bpRoadContainer);
            
            _bpPointGroupListPresenter = new BattlePassPointGroupListPresenter(_presenter.GetPoints(), _listView, _presenter.GetPointIcon(), _logger);
            _bpPointGroupListPresenter.Initialize();
            
            battlePassInfoPopup.Initialize(_audioService);
            battlePassInfoPopup.SetActive(false);
            
            battlePassPurchasePopup.Initialize();
            battlePassPurchasePopup.SetActive(false);
        }

        private void OnShowComplete() => Subscribe();

        private void InitValues()
        {
            _bpActivateBtn.SetEnabled(true);
            InitAppearanceAnimation();
            EventTimerTick(_presenter.GetTimeLeft());
            
            _bpPointIcon.style.backgroundImage = new StyleBackground(_presenter.GetPointIcon());
            _controlPointIconHolder.style.backgroundImage = new StyleBackground(_presenter.GetPointIcon());
            
            int pointInlineIndex = _presenter.GetPointInlineIndex();
            _conditionLabel.text = $"Collect <sprite index={pointInlineIndex}> by destroying enemies!";
            
            CheckActivateBtnState();
            OnObjectiveChanged();
            OnProgressChanged();
            OnProgressTextChanged();
            OnRoadProgressChanged();
        }

        private void OnProgressTextChanged()
        {
            if (_bpPointsProgressBar != null)
            {
                _bpPointsProgressBar.title = _presenter.GetProgressTitle();
            }
        }

        private void OnProgressChanged()
        {
            if (_bpPointsProgressBar != null)
            {
                _bpPointsProgressBar.value = _presenter.GetProgressValue();
            }
        }

        private void OnObjectiveChanged()
        {
            if (_controlPointLabel != null)
            {
                _controlPointLabel.text = _presenter.GetNextPointNumberText();

                if (_presenter.GetPoints().All(x => x.IsReady))
                {
                    _controlPointContainer.AddToClassList(CONTROL_POINT_ACTIVE_STYLE_NAME);
                    _logger.Log("CHANGE MAIN CONTROL POINT STYLE");
                }
            }
        }

        private void EventTimerTick(float timeLeftSeconds)
        {
            var info =
                $"Event ends in: <color=#00FF00>{TimeSpan.FromSeconds(timeLeftSeconds).ToCondensedTimeFormat()}</color>";
            _timerLabel.text = info;
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

        private void Subscribe()
        {
            _logger.Log("BP SUBSCRIBE", DebugStatus.Info);
            
            if (_exitButton1 != null)
                _exitButton1.clicked += OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked += OnExitButtonClicked;
            if (_bpActivateBtn != null)
                _bpActivateBtn.clicked += OnBpActivateBtnClicked;
            if (_bpInfoBtn != null)
                _bpInfoBtn.clicked += OnBpInfoClicked;

            _presenter.EventTimerTick += EventTimerTick;
            _presenter.Purchased += OnLeaderPassPurchased;
            
            _presenter.RoadProgressChanged += OnRoadProgressChanged;
            _presenter.ObjectiveChanged += OnObjectiveChanged;
            _presenter.ProgressChanged += OnProgressChanged;
            _presenter.ProgressTextChanged += OnProgressTextChanged;
        }

        private void Unsubscribe()
        {
            _logger.Log("BP UNSUBSCRIBE", DebugStatus.Info);
            
            _presenter.Purchased -= OnLeaderPassPurchased;
            _presenter.RoadProgressChanged -= OnRoadProgressChanged;
            
            _presenter.ObjectiveChanged -= OnObjectiveChanged;
            _presenter.ProgressChanged -= OnProgressChanged;
            _presenter.ProgressTextChanged -= OnProgressTextChanged;

            if (_exitButton1 != null)
                _exitButton1.clicked -= OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked -= OnExitButtonClicked;
            if (_bpActivateBtn != null)
                _bpActivateBtn.clicked -= OnBpActivateBtnClicked;
            if (_bpInfoBtn != null)
                _bpInfoBtn.clicked -= OnBpInfoClicked;
            
            _presenter.EventTimerTick -= EventTimerTick;
        }

        private void OnLeaderPassPurchased() => CheckActivateBtnState();

        private void CheckActivateBtnState()
        {
            if (_presenter.IsPurchased())
            {
                _bpActivateBtn.style.display = DisplayStyle.None;
                return;
            }
            
            _bpActivateBtn.style.display = DisplayStyle.Flex;
        }

        private void OnBpInfoClicked()
        {
            battlePassInfoPopup.SetActive(true);
            battlePassInfoPopup.ShowSequence();
            PlayButtonSound();
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();

        private void OnRoadProgressChanged()
        {
            if (_bpRoadProgressBar != null)
            {
                _bpRoadProgressBar.Value = _presenter.GetRoadProgressValue();
                _logger.Log($"Road progress value {_presenter.GetRoadProgressValue()}", DebugStatus.Warning);
            } 
            else
            {
                _logger.Log("Road progress bar is null", DebugStatus.Warning);
            }
        }
        
        private void OnBpActivateBtnClicked()
        {
            _audioService.PlayButtonSound();
            battlePassPurchasePopup.Show();
        }

        public void Dispose()
        {
            battlePassPurchasePopup.Dispose();
            
            battlePassInfoPopup.Dispose();
            _bpPointGroupListPresenter.Dispose();
            
            _presenter.Dispose();
            Unsubscribe();
            _animation?.Dispose();
        }

        public async UniTask<bool> AwaitForExit()
        {
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        [Button]
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