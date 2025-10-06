using _Game.UI._Shop.Scripts._DecorAndUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private DynamicGridLayout _dynamicGridLayout;
        [SerializeField] private GridLayoutGroup _gridLayout;

        private int _count;

        public Transform Transform => _transform;

        public void AddCard()
        {
            ++_count;

            if (_count > 3)
            {
                _gridLayout.childAlignment = TextAnchor.UpperCenter;
            }
            else
            {
                _gridLayout.childAlignment = TextAnchor.UpperLeft;
            }

            //UnityEngine.Debug.Log($"Cards count: {_count}");
        }

        public void RemoveCards() => _count = 0;

        private void OnDisable()
        {
            RemoveCards();
        }
    }
}