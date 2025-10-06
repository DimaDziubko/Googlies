using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils.Popups
{
    [RequireComponent(typeof(Canvas))]
    public class AlertPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
    
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private ThemedButton _okButton;
        
        [SerializeField] private Button[] _cancelButtons;

        [SerializeField] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;

        public void Construct(
            Camera uICamera,
            IAudioService audioService)
        {
            _canvas.worldCamera = uICamera;
            _canvas.enabled = false;
            _audioService = audioService;
            Subscribe();
        }

        private void Subscribe()
        {
            _okButton.onClick.AddListener(OnAccept);
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
        }

        public async UniTask<bool> AwaitForDecision(string text)
        {
            _text.text = text;
            _animation.PlayShow(OnShowComplete);
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void OnAccept()
        {
            Cleanup();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideCompleteAccept);
        }

        private void OnCancelled()
        {
            Cleanup();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideCompleteCanceled);
        }
        
        private void OnHideCompleteAccept()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }

        private void OnHideCompleteCanceled()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(false);
        }

        private void Cleanup()
        {
            _okButton.onClick.RemoveListener(OnAccept);
            
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveListener(OnCancelled);
            }
        }
    }
}