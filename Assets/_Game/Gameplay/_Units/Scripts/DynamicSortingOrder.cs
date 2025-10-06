using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class DynamicSortingOrder : MonoBehaviour
    {
        [SerializeField, Required] private Transform _sortingPoint;

        protected Vector3 Position => _sortingPoint.position;
        
        public void GameUpdate()
        {
            float sortingOrder = -Position.y / Constants.SortingLayer.SORTING_TRESHOLD;
            int newOrder = Mathf.RoundToInt(sortingOrder);
            newOrder = Mathf.Clamp(newOrder, Constants.SortingLayer.SORTING_ORDER_MIN, Constants.SortingLayer.SORTING_ORDER_MAX);

            ApplySortingOrder(newOrder);
        }

        protected abstract void ApplySortingOrder(int order);
    }
}