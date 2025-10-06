using System.Collections;
using _Game.Core.Services._Camera;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay._Units.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        private const float EPSILON = 0.01f;
        private const int INT_VIEW_THRESHOLD = 1;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _effectBar;
        [Space]
        [SerializeField] private float _effectDelay = 0.5f;
        [SerializeField] private float _effectDuration = 1.0f;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private RectTransform _rectTransform;

        private Coroutine _effectBarCoroutine;

        public Quaternion Rotation
        {
            set => _rectTransform.rotation = value;
        }

        public void Construct(IWorldCameraService cameraService)
        {
            _canvas.worldCamera = cameraService.MainCamera;
        }

        public void RotateHealthBarToCamera() => 
            Rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        public void Show()
        {
            if (_canvas == null) return;
            if (_canvas.enabled) return;
            _canvas.enabled = true;
        }

        public void Hide()
        {
            if (_canvas == null) return;
            if (!_canvas.enabled) return;
            _canvas.enabled = false;
        }

        public void UpdateHealthView(float currentHealth, float maxHealth)
        {
            if (_healthBar == null || _effectBar == null)
            {
                return;
            }

            if (_label)
            {
                _label.text = currentHealth < INT_VIEW_THRESHOLD
                    ? INT_VIEW_THRESHOLD.ToString()
                    : currentHealth.ToCompactFormat(1000);
            }

            float targetValue = Mathf.Clamp01(currentHealth / maxHealth);

            if (_effectBarCoroutine != null)
            {
                StopCoroutine(_effectBarCoroutine);
                _effectBar.value = _healthBar.value;
            }

            _healthBar.value = targetValue;

            if (gameObject.activeInHierarchy)
            {
                _effectBarCoroutine = StartCoroutine(UpdateEffectBarWithDelay(targetValue, _effectDelay));
            }
        }

        private IEnumerator UpdateEffectBarWithDelay(float targetValue, float delay)
        {
            yield return new WaitForSeconds(delay);

            while (_effectBar != null && Mathf.Abs(_effectBar.value - targetValue) > EPSILON)
            {
                _effectBar.value = Mathf.MoveTowards(_effectBar.value, targetValue, Time.deltaTime / _effectDuration);
                yield return null;
            }

            if (_effectBar != null)
            {
                _effectBar.value = targetValue;
            }
        }
    }
}