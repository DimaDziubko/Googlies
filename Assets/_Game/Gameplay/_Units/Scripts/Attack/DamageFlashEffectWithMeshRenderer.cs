using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Attack
{
    public class DamageFlashEffectWithMeshRenderer : DamageFlashEffect
    {
        [SerializeField] private MeshRenderer[] _meshRenderers;

        protected override void InitializeRenderers()
        {
            if (_meshRenderers == null || _meshRenderers.Length == 0)
            {
                _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            }
            
            _renderers = _meshRenderers;
        }

#if UNITY_EDITOR
        [Button]
        private void ManualInit()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        }
#endif
    }
}