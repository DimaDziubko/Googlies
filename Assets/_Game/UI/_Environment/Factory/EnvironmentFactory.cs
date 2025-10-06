using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Loading;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Battle.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._Environment.Factory
{
    public interface IEnvironmentFactory
    {
        UniTask<BattleEnvironment> Get(string key);
        void Reclaim(BattleEnvironment environment);
        void Cleanup();
    }

    [CreateAssetMenu(fileName = "Environment Factory", menuName = "Factories/Environment")]
    public class EnvironmentFactory : GameObjectFactory, IEnvironmentFactory
    {
        [SerializeField] private EnvironmentSocket _socketPrefab;
        
   private IWorldCameraService _cameraService;

        private EnvironmentSocket _socketObject;
        
        [ShowInInspector]
        private readonly Dictionary<string, BattleEnvironment> _environmentCache = new();
        
        public void Initialize(
            IWorldCameraService cameraService,
            EnvironmentFactoryMediator mediator)
        {
            _cameraService = cameraService;
            mediator.Initialize(this);
        }
        
        public async UniTask<BattleEnvironment> Get(string key)
        {
            if (_environmentCache.TryGetValue(key, out var cachedEnvironment) && cachedEnvironment != null)
            {
                return cachedEnvironment;
            }

            if (key.EndsWith("_SP"))
            {
                var instance = await CreateGameObjectInstanceAsync<BattleEnvironment>(key);
                instance.OriginFactory = this;

                _environmentCache[key] = instance;
                return instance;
            }
            
            if (_socketObject == null) _socketObject = GetSocket();
            var normalInstance = await CreateGameObjectInstanceAsync<BattleEnvironment>(key, _socketObject.EnvironmentAnchor);
            normalInstance.OriginFactory = this;

            _environmentCache[key] = normalInstance;
            return normalInstance;
        }

        private EnvironmentSocket GetSocket()
        {
            var instance = CreateGameObjectInstance(_socketPrefab);
            instance.OriginFactory = this;
            instance.Construct(_cameraService.MainCamera);
            return instance;
        }
        
        public void Reclaim(BattleEnvironment environment)
        {
            if (environment != null)
            {
                _environmentCache.Remove(environment.name);
                Destroy(environment.gameObject);
            }
        }
        

        public override void Cleanup()
        {
            foreach (var environment in _environmentCache.Values)
            {
                if (environment != null)
                {
                    Destroy(environment.gameObject);
                }
            }

            _environmentCache.Clear();

            if (_socketObject != null)
            {
                Destroy(_socketObject.gameObject);
                _socketObject = null;
            }

            base.Cleanup();
        }
    }
}