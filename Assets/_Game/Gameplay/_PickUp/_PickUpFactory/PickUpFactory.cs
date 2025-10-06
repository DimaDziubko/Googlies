using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Services.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._PickUp._PickUpFactory
{
    [CreateAssetMenu(fileName = "PickUp Factory", menuName = "Factories/PickUp")]
    public class PickUpFactory : GameObjectFactory, IPickUpFactory
    {
        [SerializeField] private PickUp pickUp;
        
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<PowerUpType, Queue<PickUpBase>> _sharedPools = new();

        private IAudioService _audioService;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public PickUp GetPickUp()
        {
            var pickUpInstance = (PickUp)Get(PowerUpType.Strength, pickUp);
            pickUpInstance.Construct(_audioService);
            return pickUpInstance;
        }

        private PickUpBase Get(PowerUpType type, PickUpBase prefab)
        {
            if (!_sharedPools.TryGetValue(type, out Queue<PickUpBase> pool))
            {
                pool = new Queue<PickUpBase>();
                _sharedPools[type] = pool;
            }

            PickUpBase instance;
            if (pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = CreateGameObjectInstance(prefab);
                instance.OriginFactory = this;
                instance.Type = type;
            }

            return instance;
        }
        
        
        public void Reclaim(PowerUpType type, PickUpBase pickUpBase)
        {
            pickUpBase.gameObject.SetActive(false);
            if (!_sharedPools.TryGetValue(type, out Queue<PickUpBase> pool))
            {
                pool = new Queue<PickUpBase>();
                _sharedPools[type] = pool;
            }
            pool.Enqueue(pickUpBase);
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _sharedPools.Values)
            {
                while (pool.Count > 0)
                {
                    PickUpBase pickUpBase = pool.Dequeue();
                    Destroy(pickUpBase.gameObject);
                }
            }
            _sharedPools.Clear();
            
            base.Cleanup();
        }
    }
}