using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BattleResultPopup.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class BattleResultPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button[] _exitButtons;
        [SerializeField] private DoubleCoinsBtn _doubleCoinsBtn;
        [SerializeField] private TMP_Text _victoryText;
        [SerializeField] private AmountListView _rewardListView;

        [SerializeField] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IMyLogger _logger;
        private IAudioService _audioService;
        private BattleResultPopupPresenter _presenter;
        
        private readonly List<AmountView> _views = new();

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger,
            BattleResultPopupPresenter presenter
            )
        {
            _presenter = presenter;
            _audioService = audioService;

            _logger = logger;
            _canvas.worldCamera = cameraService.UICameraOverlay;

            Init();
        }

        private void Init()
        {
            OnStateChanged();

            foreach (var button in _exitButtons)
                button.onClick.AddListener(OnCancelled);

            _doubleCoinsBtn.ButtonClicked += OnDoubleCoinsClicked;
            _presenter.StateChanged += OnStateChanged;
            _presenter.RewardVideoComplete += OnVideoComplete;
            
            foreach (var cell in _presenter.GetAdditionalRewards())
            {
                if (cell.Amount > 0)
                {
                    var view  = _rewardListView.SpawnElement();
                    view.SetIcon(cell.Icon);
                    view.SetAmount(cell.Amount.ToCompactFormat());
                    _views.Add(view);
                }
            }
        }

        private void OnDoubleCoinsClicked()
        {
            _audioService.PlayButtonSound();
            _presenter.OnDoubleCoinsClicked();
        }

        private void OnStateChanged() =>
            _doubleCoinsBtn.SetInteractable(_presenter.IsAdsButtonInteractable);

        private void Dispose()
        {
            foreach (var view in _views)
            {
                _rewardListView.DestroyElement(view);
            }
            
            _rewardListView.Clear();
            _views.Clear();
            
            foreach (var button in _exitButtons)
            {
                button.onClick.RemoveListener(OnCancelled);
            }

            _presenter.StateChanged -= OnStateChanged;
            _doubleCoinsBtn.ButtonClicked -= OnDoubleCoinsClicked;
            _presenter.RewardVideoComplete -= OnVideoComplete;
        }

        private void OnVideoComplete()
        {
            Dispose();
            _animation.PlayHide(OnHideComplete);
        }

        private void OnCancelled()
        {
            Dispose();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }

        private void OnHideComplete()
        {
            Dispose();

            _canvas.enabled = false;
            _presenter.TryToShowInterstitial();
            _taskCompletion.TrySetResult(false);
        }

        public async UniTask<bool> ShowAndAwaitForExit(GameResultType result)
        {
            if (_victoryText == null) return true;

            _victoryText.enabled = false;

            if (result == GameResultType.Victory)
            {
                _victoryText.enabled = true;
                _audioService.PlayVictorySound();
            }

            _canvas.enabled = true;

            _animation.PlayShow();

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var isExit = await _taskCompletion.Task;
            return isExit;
        }
    }
}