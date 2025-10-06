using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Settings.Scripts
{
    public class RestoreHintPopup : MonoBehaviour
    {
        [SerializeField] private Button[] _cancelButtons;
        [SerializeField] private PopupAppearanceAnimation _animation;
        
        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _animation.PlayShow(OnShowComplete);
        }

        private void OnShowComplete()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
        }
        
        private void OnCancelled()
        {
            Cleanup();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideCompleteCanceled);
            
        }
        
        private void OnHideCompleteCanceled()
        {
            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveListener(OnCancelled);
            }
            
            _animation.Cleanup();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}