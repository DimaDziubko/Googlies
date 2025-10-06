using System.Collections.Generic;
using _Game.Gameplay._BattleField.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class UnitEventDispatcher : MonoBehaviour
    {
        [ShowInInspector]
        private readonly List<IUnitEventsObserver> _eventObservers = new();

        public void RegisterEventObserver(IUnitEventsObserver observer)
        {
            if (observer == null)
            {
                return;
            }

            if (_eventObservers.Contains(observer))
            {
                return;
            }

            _eventObservers.Add(observer);
        }

        public void UnregisterEventObserver(IUnitEventsObserver observer)
        {
            if (observer == null)
            {
                return;
            }

            if (!_eventObservers.Contains(observer))
            {
                return;
            }

            _eventObservers.Remove(observer);
        }

        public void NotifyDeath(UnitBase unit)
        {
            foreach (var observer in _eventObservers)
            {
                observer.NotifyDeath(unit);
            }
        }

        public void NotifyHit(UnitBase unit, float damage)
        {
            foreach (var observer in _eventObservers)
            {
                observer.NotifyHit(unit, damage);
            }
        }
        
        public void OnPushOut(UnitBase unit)
        {
            foreach (var observer in _eventObservers)
            {
                observer.OnPushOut(unit);
            }
        }

        public void Cleanup()
        {
            _eventObservers.Clear();
        }
    }
}