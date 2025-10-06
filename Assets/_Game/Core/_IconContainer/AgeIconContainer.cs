using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core._IconContainer
{
    public class AgeIconContainer : ResourceContainer<string, ResourceContainer<string, Sprite>>
    {
        
    }
    
    public class AmbienceContainer : ResourceContainer<string, AudioClip>
    {
        
    }
    
    public class WarriorIconContainer : ResourceContainer<string, ResourceContainer<string, Sprite>>
    {
        
    }
    
    public class ShopIconsContainer : ResourceContainer<string, ResourceContainer<string, Sprite>>
    {
        
    }
    
    public class ResourceContainer<TKey, TValue>
    {
        [ShowInInspector]
        private readonly Dictionary<TKey, TValue> _resources = new();
        
        public bool TryAdd(TKey key, TValue value)
        {
            if (_resources.ContainsKey(key))
            {
                return false;
            }
            _resources[key] = value;
            return true;
        }
        
        public TValue Get(TKey key)
        {
            _resources.TryGetValue(key, out var value);
            return value;
        }
        
        public bool TryRemove(TKey key)
        {
            return _resources.Remove(key);
        }
        
        public void Clear()
        {
            _resources.Clear();
        }
        
        public bool Contains(TKey key)
        {
            return _resources.ContainsKey(key);
        }
        
        public int Count => _resources.Count;
        
        public IEnumerable<TKey> Keys => _resources.Keys;
        
        public IEnumerable<TValue> Values => _resources.Values;
        
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            if (!_resources.TryGetValue(key, out var value))
            {
                value = factory(key);
                _resources[key] = value;
            }
            return value;
        }
    }
}