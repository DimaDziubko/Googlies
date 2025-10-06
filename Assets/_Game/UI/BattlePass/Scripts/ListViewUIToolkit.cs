using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public abstract class ListViewUIToolkit<T> : MonoBehaviour where T : VisualElement
    {
        [SerializeField, Required]
        private VisualTreeAsset _itemPrefab;

        private VisualElement _container;

        private readonly List<T> _items = new();
        private readonly Queue<T> _freeList = new();

        public void Initialize(VisualElement container)
        {
            _container = container;
        }

        public T SpawnElement()
        {
            T item;

            if (_freeList.Count > 0)
            {
                item = _freeList.Dequeue();
                item.style.display = DisplayStyle.Flex;
            }
            else
            {
                var visualElement = _itemPrefab.CloneTree();
                _container.Add(visualElement);
                item = visualElement.Q<T>();
            }

            _items.Add(item);
            return item;
        }

        public void DestroyElement(T item)
        {
            if (item != null && _items.Remove(item))
            {
                item.style.display = DisplayStyle.None;
                _freeList.Enqueue(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                item.style.display = DisplayStyle.None;
                _freeList.Enqueue(item);
            }

            _items.Clear();
        }

        public void MarkDirtyRepaint()
        {
            _container?.MarkDirtyRepaint();
        }
    }
}