using System.Collections.Generic;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._Logger;
using _Game.Core.Factory;
using _Game.Core.Loading;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Bases.Factory
{
    [CreateAssetMenu(fileName = "Base Factory", menuName = "Factories/Base")]
    public class BaseFactory : GameObjectFactory, IBaseFactory
    {
        private IBaseDataProvider _baseDataProvider;
        private IWorldCameraService _cameraService;
        private ITargetRegistry _targetRegistry;
        private IMyLogger _logger;
        
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<string, Base> _baseCache = new();

        public void Initialize(
            ITargetRegistry targetRegistry,
            IBaseDataProvider baseDataProvider,
            IWorldCameraService cameraService,
            IMyLogger logger,
            BaseFactoryMediator mediator)
        {
            _targetRegistry = targetRegistry;
            _baseDataProvider = baseDataProvider;
            _cameraService = cameraService;
            _logger = logger;
            mediator.Initialize(this);
        }
        
        public async UniTask<Base> GetAsync(Faction faction)
        {
            IBaseData baseData = _baseDataProvider
                .GetBaseData(faction);

            var key = $"{faction}_{baseData.BaseAssetReference.AssetGUID}";
            
            if (_baseCache.TryGetValue(key, out Base existingBase))
            {
                if (!existingBase.gameObject.activeSelf)
                {
                    existingBase.gameObject.SetActive(true);
                    existingBase.Reset();
                    existingBase.Init();
                }
                return existingBase;
            }
            
            var assetReference = baseData.BaseAssetReference;
            Base instance = await CreateGameObjectInstanceAsync<Base>(assetReference);

            instance.OriginFactory = this;
            instance.Key = key;

            instance.Construct(
                _targetRegistry,
                baseData.Health, 
                baseData.CoinsAmount, 
                _cameraService,
                faction,
                baseData.Layer);

            instance.Init();
            
            _baseCache[key] = instance;
            return instance;
        }
        
        public void Reclaim(Base @base)
        {
            @base.gameObject.SetActive(false); 
        }
        
        public override void Cleanup()
        {
            foreach (var instance in _baseCache.Values)
            {
                Destroy(instance.gameObject);
            }
            
            _baseCache.Clear();
            
            _logger.Log("BASE CLEANUP", DebugStatus.Warning);
            
            base.Cleanup();
        }
    }
}