using System.Collections.Generic;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonResultPopup : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private TMP_Text _nameLabel;
        [SerializeField, Required] private Button[] _exitButtons;
        [SerializeField, Required] private ThemedButton _collectButton;

        [SerializeField, Required] private AmountListView _rewardListView;

        [SerializeField, Required] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private DungeonResultPopupPresenter _presenter;

        private readonly List<AmountView> _views = new();
        
        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            DungeonResultPopupPresenter presenter)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = presenter;

            _audioService = audioService;

            Init();
        }

        private void Init()
        {
            OnStateChanged();

            foreach (var button in _exitButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }

            _nameLabel.text = _presenter.GetDungeonName();
            _collectButton.onClick.AddListener(OnCollect);
        }

        private void OnStateChanged()
        {
            AmountView mainRewardView = _rewardListView.SpawnElement();
            mainRewardView.SetIcon(_presenter.GetRewardIcon());
            mainRewardView.SetAmount(_presenter.GetRewardAmount());
            _views.Add(mainRewardView);
            
            foreach (var cell in _presenter.GetAdditionalRewards())
            {
                if (cell.Amount > 0)
                {
                    var view  = _rewardListView.SpawnElement();
                    view.SetIcon(_presenter.GetIconFor(cell.Type));
                    view.SetAmount(cell.Amount.ToCompactFormat());
                    _views.Add(view);
                }
            }
        }

        private void OnCollect()
        {
            Dispose();
            _presenter.OnCollect();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }

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

            _collectButton.onClick.RemoveListener(OnCollect);
        }

        public async UniTask<bool> AwaitForDecision()
        {
            _canvas.enabled = true;
            _animation.PlayShow();

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnCancelled() => OnCollect();

        private void OnHideComplete()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(false);
        }
    }
}