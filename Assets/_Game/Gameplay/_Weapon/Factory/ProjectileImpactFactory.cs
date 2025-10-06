using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Weapon._Projectile;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Factory
{
    public enum ProjectileImpactType
    {
        None,
        Direct,
        Splash
    }
    
    [CreateAssetMenu(fileName = "Projectile Impact Factory", menuName = "Factories/Projectile Impact")]
    public class ProjectileImpactFactory : ScriptableObject
    {
        private ITargetRegistry _targetRegistry;

        public void Initialize(ITargetRegistry targetRegistry)
        {
            _targetRegistry = targetRegistry;
        }
        
        public ProjectileImpact Get(ProjectileImpactType type)
        {
            return type switch
            {
                ProjectileImpactType.Direct => new DirectProjectileImpact(),
                ProjectileImpactType.Splash => new SplashProjectileImpact(_targetRegistry),
                _ => new DirectProjectileImpact(),
            };
        }
    }
}