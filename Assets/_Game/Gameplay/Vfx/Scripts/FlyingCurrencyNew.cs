using System.Collections;
using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using Coffee.UIExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class FlyingCurrencyNew : FlyingReward
    {
        [SerializeField, Required] private ParticleSystem _particles;
        [SerializeField, Required] private RectTransform _rectTransform;
        [SerializeField, Required] private CurrencyType _currencyType;
        [SerializeField, Required] private float _lifetime = 8f;
        
        private Coroutine _lifetimeCoroutine;
        private UIParticleAttractor _attractor;
        public CurrencyType CurrencyType => _currencyType;
        public CurrencyVfxRenderMode RenderMode {get; set; }

        public void Construct(UIParticleAttractor attractor)
        {
            _attractor = attractor;
        }

        public void Initialize(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
            _attractor.AddParticleSystem(_particles);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void Launch()
        {
            if (_lifetimeCoroutine != null)
                StopCoroutine(_lifetimeCoroutine);

            _lifetimeCoroutine = StartCoroutine(AutoRecycle(_lifetime));
        }

        private IEnumerator AutoRecycle(float delay)
        {
            yield return new WaitForSeconds(delay);

            _lifetimeCoroutine = null;
            _attractor.RemoveParticleSystem(_particles);
            OriginFactory.Reclaim(this);
        }

        private void CancelLifetime()
        {
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }
        }

        public override void Recycle()
        {
            CancelLifetime();
            base.Recycle();
        }
    }
}