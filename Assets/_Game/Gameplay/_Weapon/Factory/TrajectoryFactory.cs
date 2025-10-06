using System.Collections.Generic;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Factory
{
    [CreateAssetMenu(fileName = "Trajectory Factory", menuName = "Factories/Trajectory")]
    public class TrajectoryFactory : ScriptableObject
    {
        [SerializeField] private AnimationCurve _ballisticCurve;
        [SerializeField] private AnimationCurve _javelinCurve;

        private readonly Dictionary<TrajectoryType, Trajectory> _cache = new(2);

        public ITrajectory Get(TrajectoryType type)
        {
            if (_cache.TryGetValue(type, out var cached))
                return cached;

            var trajectory = type switch
            {
                TrajectoryType.Ballistic => new Trajectory(_ballisticCurve),
                TrajectoryType.Javelin => new Trajectory(_javelinCurve),
                _ => new Trajectory(_ballisticCurve)
            };

            _cache[type] = trajectory;
            return trajectory;
        }
    }
}