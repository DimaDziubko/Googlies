using _Game.Core.Services.Audio;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Factory
{
    public enum WeaponType
    {
        None,
        SimpleMelee,
        SimpleProjectile,
        RangeNonProjectile,
        LaserSaber,
        LaserWhip,
        PushMelee,
    }

    [CreateAssetMenu(fileName = "Weapon Factory", menuName = "Factories/Weapon")]
    public class WeaponFactory : ScriptableObject
    {
        [SerializeField, Required] private TrajectoryFactory _trajectoryFactory;
        
        private ISoundService _soundService;

        public void Initialize(ISoundService soundService)
        {
            _soundService = soundService;
        }
        
        public WeaponStrategy Get(WeaponType type)
        {
            return type switch
            {
                WeaponType.SimpleMelee => new SimpleMelee(_soundService),
                WeaponType.SimpleProjectile => new SimpleProjectile(_soundService, _trajectoryFactory),
                WeaponType.LaserSaber => new LightSaber(_soundService),
                WeaponType.LaserWhip => new LightWhip(_soundService),
                WeaponType.RangeNonProjectile => new RangeNonProjectile(_soundService),
                WeaponType.PushMelee => new PushMelee(_soundService),
                _ => new SimpleMelee(_soundService),
            };
        }
    }
}