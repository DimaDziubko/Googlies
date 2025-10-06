using System;
using UnityEngine;
using Unity.Theme.Binders;
using Cysharp.Threading.Tasks;
using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;

namespace _Game.UI.RateGame.Scripts
{
    public class RateGameScreen : MonoBehaviour
    {
        public event Action OnClose;
        public event Action OnRateGame;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private ThemedButton _rateGameButton;
        [SerializeField] private ThemedButton _noCloseButton;


        private UniTaskCompletionSource<bool> _taskCompletion;
        private IAudioService _audioService;
        private IMyLogger _logger;

        public void Construct(IWorldCameraService cameraService, IAudioService audioService, IMyLogger logger)
        {
            _logger = logger;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _audioService = audioService;

            Unsubscribe();
            Subscribe();
        }

        public async UniTask<bool> AwaitForDecision()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        public void Close() => _taskCompletion.TrySetResult(true);

        private void Subscribe()
        {
            _noCloseButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                PlayButtonSound();
            });

            _rateGameButton.onClick.AddListener(() =>
            {
                OnRateGame?.Invoke();
                PlayButtonSound();
            });
        }

        private void Unsubscribe()
        {
            _noCloseButton.onClick.RemoveAllListeners();
            _rateGameButton.onClick.RemoveAllListeners();
        }
        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();
    }
}