using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class DamageFlashEffectWithSpriteRenderer : DamageFlashEffect
    {
        [SerializeField] private SpriteRenderer[] _spriteRenderers;

        protected override void InitializeRenderers()
        {
            if (_spriteRenderers == null || _spriteRenderers.Length == 0)
            {
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            }

            _renderers = _spriteRenderers;
        }

#if UNITY_EDITOR
        [Button]
        protected override void ManualInit()
        {
            base.ManualInit();
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
#endif
    }
}