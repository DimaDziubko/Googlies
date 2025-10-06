using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._Shop.Scripts.Common
{
    public abstract class ListView<T> : MonoBehaviour where T : Component
    {
        [SerializeField, Required] private T itemPrefab;
        [SerializeField, Required] private Transform container;
        
        private readonly List<T> _items = new();
        private readonly Queue<T> _freeList = new();

        public T SpawnElement()
        {
            if (_freeList.TryDequeue(out var item))
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(itemPrefab, container);
            }
            
            _items.Add(item);
            return item;
        }

        public void DestroyElement(T item)
        {
            if (item != null && _items.Remove(item))
            {
                item.gameObject.SetActive(false);
                _freeList.Enqueue(item);
            }
        }
        
        public void Clear()
        {
            for (int i = 0, count = _items.Count; i < count; i++)
            {
                T item = _items[i];
                item.gameObject.SetActive(false);
                _freeList.Enqueue(item);
            }
            
            _items.Clear();
        }
    }
}