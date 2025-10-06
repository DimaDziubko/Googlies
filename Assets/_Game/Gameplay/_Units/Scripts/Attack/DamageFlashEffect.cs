using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public abstract class DamageFlashEffect : MonoBehaviour
    {
        private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

        [SerializeField, Required] private Health _health;
        [SerializeField] private float _flashTime = 0.25f;

        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private AnimationCurve _flashSpeedCurve;

        private Coroutine _damageFlashCoroutine;
        
        
        protected Renderer[] _renderers;
        private MaterialPropertyBlock _propBlock;

        public void Initialize()
        {
            _health.Hit -= CallDamageFlash;
            _health.Hit += CallDamageFlash;
            
            InitializeRenderers();
            _propBlock = new MaterialPropertyBlock();
        }

        public void Cleanup()
        {
            _health.Hit -= CallDamageFlash;
        }

        public void Reset()
        {
            SetFlashAmount(0);
        }
        
        protected abstract void InitializeRenderers();

        private void SetFlashColor()
        {
            foreach (var renderer in _renderers)
            {
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor(FlashColor, _flashColor);
                renderer.SetPropertyBlock(_propBlock);
            }
        }

        private void CallDamageFlash(float _, float __)
        {
            if (!gameObject.activeInHierarchy || _renderers == null)
            {
                return;
            }

            if (_damageFlashCoroutine != null)
            {
                StopCoroutine(_damageFlashCoroutine);
            }

            _damageFlashCoroutine = StartCoroutine(DamageFlasher());
        }

        private IEnumerator DamageFlasher()
        {
            SetFlashColor();
            float elapsedTime = 0f;

            while (elapsedTime < _flashTime)
            {
                elapsedTime += Time.deltaTime;
                float currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), elapsedTime / _flashTime);
                SetFlashAmount(currentFlashAmount);
                yield return null;
            }
        }

        private void SetFlashAmount(float amount)
        {
            foreach (var renderer in _renderers)
            {
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(FlashAmount, amount);
                renderer.SetPropertyBlock(_propBlock);
            }
        }
        
#if UNITY_EDITOR
        [Button]
        protected virtual void ManualInit()
        {
            _health = GetComponent<Health>();
        }
#endif
        
    }
}