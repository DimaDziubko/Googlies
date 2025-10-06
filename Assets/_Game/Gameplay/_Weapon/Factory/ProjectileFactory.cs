using System.Collections.Generic;
using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._Weapon.Factory
{
    public interface IProjectileFactory
    {
        UniTask<Projectile> GetAsync(Faction faction, int weaponId);
        UniTask<Projectile> GetAsync(Faction faction, IWeaponData weaponData);
        public void Reclaim(Projectile proj);
    }

    [CreateAssetMenu(fileName = "Projectile Factory", menuName = "Factories/Projectile")]
    public class ProjectileFactory : GameObjectFactory, IProjectileFactory
    {
        [SerializeField, Required] private ProjectileMoveFactory _projectileMoveFactory;
        [SerializeField, Required] private ProjectileImpactFactory _projectileImpactFactory;

        private IWeaponDataProvider _weaponDataProvider;
        private ISoundService _soundService;
        private ITargetRegistry _targetRegistry;
        
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<(Faction, int), Queue<Projectile>> _projectilesPools = new();

        public void Initialize(
            ITargetRegistry targetRegistry,
            ISoundService soundService,
            IWeaponDataProvider weaponDataProvider)
        {
            _targetRegistry = targetRegistry;
            _weaponDataProvider = weaponDataProvider;
            _soundService = soundService;
            
            _projectileImpactFactory.Initialize(targetRegistry);
        }
        
        public async UniTask<Projectile> GetAsync(Faction faction,  int weaponId)
        {
            IWeaponData weaponData = GetWeaponData(faction, weaponId);
            if (weaponData == null) return null;
            if (weaponData.ProjectileKey == Constants.ConfigKeys.MISSING_KEY) return null;

            if (!_projectilesPools.TryGetValue((faction, weaponId), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(faction, weaponId)] = pool;
            }
            
            Projectile instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                await UniTask.DelayFrame(1);
                instance = await CreateGameObjectInstanceAsync<Projectile>(weaponData.ProjectileKey);
                instance.OriginFactory = this;
            }

            instance.Construct(
                _soundService, 
                faction,
                weaponData, 
                _projectileMoveFactory.Get(weaponData.TrajectoryType),
                _projectileImpactFactory.Get(weaponData.ProjectileImpactType));
            
            return instance;
        }

        public async UniTask<Projectile> GetAsync(Faction faction, IWeaponData weaponData)
        {
            if (weaponData == null) return null;
            if (weaponData.ProjectileKey == Constants.ConfigKeys.MISSING_KEY) return null;

            if (!_projectilesPools.TryGetValue((faction, weaponData.WeaponId), out Queue<Projectile> pool))
            {
                pool = new Queue<Projectile>();
                _projectilesPools[(faction, weaponData.WeaponId)] = pool;
            }
            
            Projectile instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                await UniTask.DelayFrame(1);
                instance = await CreateGameObjectInstanceAsync<Projectile>(weaponData.ProjectileKey);
                instance.OriginFactory = this;
            }

            instance.Construct(
                _soundService,
                faction,
                weaponData,
                _projectileMoveFactory.Get(weaponData.TrajectoryType),
                _projectileImpactFactory.Get(weaponData.ProjectileImpactType));
            
            return instance;
        }
        
        private IWeaponData GetWeaponData(Faction faction, int weaponId) => 
            _weaponDataProvider.GetWeaponData(weaponId, faction);

        public void Reclaim(Projectile proj)
        {
            if (proj != null || proj.OrNull() != null)
            {
                if (!_projectilesPools.TryGetValue((proj.Faction, proj.WeaponId), out Queue<Projectile> pool))
                {
                    pool = new Queue<Projectile>();
                    _projectilesPools[(proj.Faction, proj.WeaponId)] = pool;
                }

                proj.gameObject.SetActive(false);
                pool.Enqueue(proj);
            }
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _projectilesPools.Values)
            {
                while (pool.Count > 0)
                {
                    var proj = pool.Dequeue();
                    Destroy(proj.gameObject);
                }
            }
            
            _projectilesPools.Clear(); 
            
            base.Cleanup();
        }
    }
}