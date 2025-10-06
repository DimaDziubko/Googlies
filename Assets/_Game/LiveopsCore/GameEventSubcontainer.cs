using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.UserState._State;
using Sirenix.OdinInspector;

namespace _Game.LiveopsCore
{
    public class GameEventSubcontainer
    {
        [ShowInInspector] private readonly Dictionary<int, GameEventBase> _events = new();

        public event Action<GameEventBase> EventAdded;
        public event Action<GameEventBase> EventRemoved;

        public bool Add(GameEventBase model)
        {
            var added = _events.TryAdd(model.Id, model);
            if (!added) _events[model.Id] = model;
            EventAdded?.Invoke(model);
            return added;
        }

        public bool Remove(int id)
        {
            if (_events.Remove(id, out var model))
            {
                EventRemoved?.Invoke(model);
                return true;
            }

            return false;
        }

        public bool Contains(int id) => _events.ContainsKey(id);
        public GameEventBase GetById(int id) => _events.GetValueOrDefault(id);
        public IReadOnlyCollection<GameEventBase> GetAll() => _events.Values;
        public IEnumerable<GameEventBase> GetByType(GameEventType type) =>
            _events.Values.Where(m => m.Type == type);
        public int Count => _events.Count;
        public void Clear() => _events.Clear();
    }
}