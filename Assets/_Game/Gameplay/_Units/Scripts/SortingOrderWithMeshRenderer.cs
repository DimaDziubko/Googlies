using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SortingOrderWithMeshRenderer : DynamicSortingOrder
    {
        [SerializeField, Required] private MeshRenderer _meshRenderer;

        protected override void ApplySortingOrder(int order)
        {
            if (_meshRenderer.sortingOrder != order)
            {
                _meshRenderer.sortingOrder = order;
            }
        }
        
#if UNITY_EDITOR
        [Button]
        private void ManualInit()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
#endif
    }
}