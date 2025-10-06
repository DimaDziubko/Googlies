using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class ImageMaskAnimation : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Image[] _images;
        [SerializeField] private Material[] _materials;

        [SerializeField] private float _hideTime = 0.5f;
        [SerializeField] private AnimationCurve _flashSpeedCurve;
        
        private Coroutine _imageFlashCoroutine;

        public void Init()
        {
            _materials = new Material[_images.Length];

            for (int i = 0; i < _images.Length; i++)
            {
                _materials[i] = new Material(_images[i].material);
                _images[i].material = _materials[i];
            }

            ImagesSetActive(true);
        }

        public void TriggerMask(Action callback = null)
        {
            if (_imageFlashCoroutine != null)
            {
                StopCoroutine(_imageFlashCoroutine);
            }

            _imageFlashCoroutine = StartCoroutine(FadeAnimation(callback));
        }

        public async UniTask TriggerMaskAsync()
        {
            float elapsedTime = 0f;
            while (elapsedTime < _hideTime)
            {
                elapsedTime += Time.deltaTime;
                var currentFadeAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _hideTime));
                SetBaseColorAlpha(currentFadeAmount);
                await UniTask.Yield();
            }

            ImagesSetActive(false);
        }
        
        private IEnumerator FadeAnimation(Action callback = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < _hideTime)
            {
                elapsedTime += Time.deltaTime;
                var currentFadeAmount =
                    Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _hideTime));
                SetBaseColorAlpha(currentFadeAmount);
                yield return null;
            }

            ImagesSetActive(false);
            
            callback?.Invoke();
        }
        
        private void SetBaseColorAlpha(float alpha)
        {
            foreach (var material in _materials)
            {
                var currentBaseColor = material.GetColor(BaseColor);
                var newColor = new Color(currentBaseColor.r, currentBaseColor.g, currentBaseColor.b, alpha);
                material.SetColor(BaseColor, newColor);
            }
        }

        private void ImagesSetActive(bool isActive)
        {
            foreach (var image in _images)
            {
                image.enabled = isActive;
            }
        }
    }
}