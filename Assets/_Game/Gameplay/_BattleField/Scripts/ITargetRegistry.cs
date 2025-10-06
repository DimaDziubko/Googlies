using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface ITargetRegistry
    {
        void Register(ITarget target);
        void Unregister(ITarget target);
    
        IEnumerable<ITarget> GetAll();
        IEnumerable<ITarget> GetAlive();
        IEnumerable<ITarget> GetEnemiesInRange(Vector3 origin, float radius, Faction selfFaction);
        bool TryGetTarget(Collider2D collider, out ITarget target);
    }
}