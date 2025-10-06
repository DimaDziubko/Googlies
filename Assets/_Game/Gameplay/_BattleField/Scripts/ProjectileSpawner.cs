using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Common;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityUtils;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class ProjectileSpawner : IShootProxy
    {
        private readonly IProjectileFactory _projectileFactory;
        private readonly IVFXProxy _vfxProxy;
        private readonly IBattleSpeedManager _battleSpeedManager;

        [ShowInInspector, ReadOnly]
        private readonly GameBehaviourCollection _projectiles = new();

        public ProjectileSpawner(
            IProjectileFactory projectileFactory,
            IVFXProxy vfxProxy,
            IBattleSpeedManager battleSpeedManager)
        {
            _projectileFactory = projectileFactory;
            _vfxProxy = vfxProxy;
            _battleSpeedManager = battleSpeedManager;
        }

        public void GameUpdate(float deltaTime)
        {
            _projectiles.GameUpdate(deltaTime);
        }

        public void SetPaused(bool isPaused)
        {
            _projectiles.SetPaused(isPaused);
        }

        public void Cleanup()
        {
            _projectiles.Clear();
        }

        async UniTask IShootProxy.Shoot(ShootData data)
        {
            Projectile projectile = await _projectileFactory.GetAsync(data.Faction, data.WeaponData);
            if(projectile.OrNull() == null) return;

            projectile.PrepareIntro(
                _vfxProxy,
                data.LaunchPosition, 
                data.Target, 
                data.LaunchRotation,
                _battleSpeedManager.CurrentSpeedFactor,
                data.ProjectileScale);
            
            _projectiles.Add(projectile);
        }

        public void SetSpeedFactor(float speedFactor) => _projectiles.SetBattleSpeedFactor(speedFactor);
    }
}