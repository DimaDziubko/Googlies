using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Factory;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Factory
{
    [CreateAssetMenu(fileName = "Unit Factory", menuName = "Factories/Unit")]
    public class UnitFactory : GameObjectFactory, IUnitFactory
    {
        [SerializeField, Required] private WeaponFactory _weaponFactory;
        
        private IWorldCameraService _cameraService;
        private IRandomService _random;
        private IUnitDataProvider _unitDataProvider;
        private ITargetRegistry _targetRegistry;
        private IMyLogger _logger;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<(Faction, UnitType), Queue<UnitBase>> _unitsPools = new();

        public void Initialize(
            IWorldCameraService cameraService,
            IRandomService random,
            ISoundService soundService,
            IUnitDataProvider unitDataProvider,
            ITargetRegistry targetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _targetRegistry = targetRegistry;
            _cameraService = cameraService;
            _random = random;
            _unitDataProvider = unitDataProvider;
            
            _weaponFactory.Initialize(soundService);
        }
        
        public async UniTask<UnitBase> GetAsync(UnitType type, Skin skin, Faction faction)
        {
            if (!_unitsPools.TryGetValue((faction, type), out Queue<UnitBase> pool))
            {
                pool = new Queue<UnitBase>();
                _unitsPools.Add((faction, type), pool);
            }

            UnitBase instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
                instance.Reset();
            }
            else
            {
                IUnitData unitData = _unitDataProvider
                    .GetDecoratedUnitData(faction, type, skin);

                instance = await ConstructUnit(faction, unitData);
            }
            
            return instance;
        }

        private async UniTask<UnitBase> ConstructUnit(Faction faction, IUnitData unitData)
        {
            var instance = await CreateGameObjectInstanceAsync<UnitBase>(unitData.PrefabKey);
            
            instance.OriginFactory = this;
            
            instance.Weapon.Construct(_weaponFactory.Get(unitData.WeaponData.WeaponType), _logger);

            instance.Health.Construct(
                unitData.Health,
                _cameraService);

            instance.AMove.Construct(
                unitData.Speed);

            instance.Positioner.Apply(
                unitData.PositionSettings);

            instance.PushComponent.Construct(unitData.IsPushable);
            
            instance.Construct(
                _logger,
                _targetRegistry,
                unitData, 
                faction,
                _random);
            
            return instance;
        }

        public void Reclaim(UnitBase unit)
        {
            unit.gameObject.SetActive(false); 
            
            var key = (unit.Faction, unit.Type);
            if (!_unitsPools.TryGetValue(key, out Queue<UnitBase> pool))
            {
                pool = new Queue<UnitBase>();
                _unitsPools.Add(key, pool);
            }
            
            pool.Enqueue(unit);
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _unitsPools.Values)
            {
                while (pool.Count > 0)
                {
                    var unit = pool.Dequeue();
                    Destroy(unit.gameObject);
                }
            }
            _unitsPools.Clear();
            
            base.Cleanup();
        }
    }
}