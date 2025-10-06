using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameListenerComposite;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.BattleResultPopup.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class TargetRegistry : ITargetRegistry, IEndGameListener
    {
        [ShowInInspector, ReadOnly]
        private readonly HashSet<ITarget> _allTargets = new();

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<Collider2D, ITarget> _colliderToTarget = new();

        public void Register(ITarget target)
        {
            _allTargets.Add(target);

            if (target.Collider != null)
            {
                _colliderToTarget[target.Collider] = target;
            }
        }

        public void Unregister(ITarget target)
        {
            _allTargets.Remove(target);

            if (target.Collider != null)
            {
                _colliderToTarget.Remove(target.Collider);
            }
        }

        public bool TryGetTarget(Collider2D collider, out ITarget target) =>
            _colliderToTarget.TryGetValue(collider, out target);

        public IEnumerable<ITarget> GetAll() => _allTargets;

        public IEnumerable<ITarget> GetAlive() =>
            _allTargets.Where(t => t.IsActive && !t.IsDead());

        public IEnumerable<ITarget> GetEnemiesInRange(Vector3 origin, float radius, Faction selfFaction)
        {
            float sqrRadius = radius * radius;

            return _allTargets
                .Where(t => t.IsActive
                            && !t.IsDead()
                            && t.Faction != selfFaction
                            && (t.Transform.position - origin).sqrMagnitude <= sqrRadius);
        }

        private void Cleanup()
        {
            _allTargets.Clear();
            _colliderToTarget.Clear();
        }

        void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit) => Cleanup();
    }
}