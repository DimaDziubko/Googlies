using _Game.Gameplay._Weapon._Projectile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Factory
{
    public enum TrajectoryType
    {
        None = 0,
        Ballistic = 1,
        Javelin = 2, 
    }
    
    [CreateAssetMenu(fileName = "Projectile Move Factory", menuName = "Factories/Projectile Move")]
    public class ProjectileMoveFactory : ScriptableObject
    {
        [SerializeField, Required] private TrajectoryFactory _trajectoryFactory;
        
        public ProjectileMove Get(TrajectoryType type) => 
            new TrajectoryMove(_trajectoryFactory.Get(type));
    }
}