using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Game.Gameplay._Units.Scripts
{
    public class SortingOrderWithSortingGroup : DynamicSortingOrder
    {
        [SerializeField, Required] private SortingGroup _sortingGroup;

        protected override void ApplySortingOrder(int order)
        {
            if (_sortingGroup.sortingOrder != order)
            {
                _sortingGroup.sortingOrder = order;
            }
        }
    }
}