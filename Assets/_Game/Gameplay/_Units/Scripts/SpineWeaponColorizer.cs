using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SpineWeaponColorizer : WeaponColorizer
    {
        private static readonly int TintColorID = Shader.PropertyToID("_Color");

        [SerializeField, Required] private Renderer[] _renderers;
        
        [SerializeField] private int[] _materialIndices = { 0 };

        private MaterialPropertyBlock _propBlock;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        public override void SetWeaponColor(Color color)
        {
            foreach (var renderer in _renderers)
            {
                foreach (var index in _materialIndices)
                {
                    if (index < 0 || index >= renderer.sharedMaterials.Length)
                    {
                        continue;
                    }

                    renderer.GetPropertyBlock(_propBlock, index);
                    _propBlock.SetColor(TintColorID, color);
                    renderer.SetPropertyBlock(_propBlock, index);
                }
            }
        }
    }
}