using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Loading;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.LoadingScreen
{
    public enum LoadingScreenType
    {
        Simple,
        Transparent,
        DarkFade,
    }

    [RequireComponent(typeof(Canvas))]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private List<Sprite> _sprites;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _progressFill;
        [SerializeField] private TextMeshProUGUI _loadingInfo;
        [SerializeField] private float _barSpeed;

        //Animation
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _simpleScreen;
        [SerializeField] private GameObject _darkScreen;

        private IMyLogger _logger;

        public void Construct(IMyLogger logger)
        {
            _logger = logger;
        }

        private float _targetProgress;

        public async UniTask Load(
            Queue<ILoadingOperation> loadingOperations, LoadingScreenType type)
        {
            switch (type)
            {
                case LoadingScreenType.Simple:
                    await SimpleLoading(loadingOperations);
                    break;
                case LoadingScreenType.Transparent:
                    await TransparentLoading(loadingOperations);
                    break;
                case LoadingScreenType.DarkFade:
                    await DarkFadeLoading(loadingOperations);
                    break;
            }
        }

        private async UniTask TransparentLoading(Queue<ILoadingOperation> loadingOperations)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, "TransparentLoading");
            perfTimer.Start();
#endif
            EnableCanvas();
            _canvasGroup.alpha = 0;
            _loadingInfo.enabled = false;
            _simpleScreen.SetActive(false);
            _darkScreen.SetActive(true);
            await LoadOperations(loadingOperations);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }

        private async UniTask SimpleLoading(Queue<ILoadingOperation> loadingOperations)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, "SimpleLoading");
            perfTimer.Start();
#endif
            SetRandomSprite();
            EnableCanvas();
            _canvasGroup.alpha = 1;
            _simpleScreen.SetActive(true);
            _darkScreen.SetActive(false);
            await LoadOperations(loadingOperations);
            await PlayFadeAnimation(1, 0, 2);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }

        private async UniTask DarkFadeLoading(Queue<ILoadingOperation> loadingOperations)
        {
            var perfTimer = new PerfTimer(_logger, "DarkFadeLoading");
            perfTimer.Start();

            EnableCanvas();
            _canvasGroup.alpha = 0;
            _simpleScreen.SetActive(false);
            _darkScreen.SetActive(true);
            await PlayFadeAnimation(0, 1, 0.25f);
            await LoadOperations(loadingOperations);
            await PlayFadeAnimation(1, 0, 0.25f);

            perfTimer.Stop();
        }

        private void SetRandomSprite()
        {
            if (_sprites != null && _sprites.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, _sprites.Count);
                _image.sprite = _sprites[randomIndex];
            }
            else
            {
                //Debug.LogWarning("Sprite collection is empty or not assigned.");
            }
        }


        private async UniTask LoadOperations(Queue<ILoadingOperation> loadingOperations)
        {
            foreach (var operation in loadingOperations)
            {
                _loadingInfo.text = operation.Description;
                await operation.Load(OnProgress);
            }
        }


        private async UniTask PlayFadeAnimation(float startValue, float endValue, float duration)
        {
            _canvasGroup.alpha = startValue;
            await _canvasGroup.DOFade(endValue, duration).AsyncWaitForCompletion();
        }

        private void EnableCanvas()
        {
            _canvas.enabled = true;
            ResetProgress();
        }

        private void ResetProgress()
        {
            _progressFill.value = 0;
            _targetProgress = 0;
        }

        private void OnProgress(float progress)
        {
            _targetProgress = progress;
        }

        private void ResetFill()
        {
            _progressFill.value = 0;
            _targetProgress = 0;
        }

        private async UniTask WaitForBarFill()
        {
            while (_progressFill.value < _targetProgress)
            {
                await UniTask.Yield();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
        }

        private IEnumerator UpdateProgressBar()
        {
            while (_canvas.enabled)
            {
                if (_progressFill.value < _targetProgress)
                    _progressFill.value += Time.deltaTime * _barSpeed;
                yield return null;
            }
        }
    }
}
