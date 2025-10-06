using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class DissolveEffect : MonoBehaviour
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        private static readonly int DissolveColor = Shader.PropertyToID("_DissolveColor");

        [SerializeField] private MeshRenderer[] _meshRenderers;
        
        [SerializeField] private float _dissolveTime = 1f;

        [ColorUsage(true, true)]
        [SerializeField] private Color _dissolveColor = Color.black;
        [SerializeField] private AnimationCurve _dissolveCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Coroutine _dissolveCoroutine;
    
        protected Renderer[] _renderers;
        private MaterialPropertyBlock _propBlock;

        public void Initialize()
        {
            InitializeRenderers();
            _propBlock = new MaterialPropertyBlock();
            SetDissolveAmount(0);
        }

        public void Cleanup()
        {
            if (_dissolveCoroutine != null)
            {
                StopCoroutine(_dissolveCoroutine);
                _dissolveCoroutine = null;
            }
        }

        public void Reset()
        {
            SetDissolveAmount(0);
        }

        private void InitializeRenderers()
        {
            if (_meshRenderers == null || _meshRenderers.Length == 0)
            {
                _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            }
            
            _renderers = _meshRenderers;
        }

        private void SetDissolveColor()
        {
            foreach (var renderer in _renderers)
            {
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor(DissolveColor, _dissolveColor);
                renderer.SetPropertyBlock(_propBlock);
            }
        }

        public void CallDissolve(Action onComplete = null)
        {
            if (!gameObject.activeInHierarchy || _renderers == null)
                return;

            if (_dissolveCoroutine != null)
            {
                StopCoroutine(_dissolveCoroutine);
                _dissolveCoroutine = null;
                onComplete?.Invoke();
            }
                

            _dissolveCoroutine = StartCoroutine(Dissolver(onComplete));
        }

        private IEnumerator Dissolver(Action onComplete = null)
        {
            SetDissolveColor();
            float elapsedTime = 0f;

            while (elapsedTime < _dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / _dissolveTime;
                float amount = _dissolveCurve.Evaluate(progress);

                SetDissolveAmount(amount);
                yield return null;
            }
            
           
            SetDissolveAmount(1f);
            _dissolveCoroutine = null;

            onComplete?.Invoke();
        }

        private void SetDissolveAmount(float amount)
        {
            foreach (var renderer in _renderers)
            {
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(DissolveAmount, amount);
                renderer.SetPropertyBlock(_propBlock);
            }
        }
        
#if UNITY_EDITOR
        [Button]
        protected virtual void ManualInit()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }
#endif
    }
}