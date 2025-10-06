using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Factory;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay.Vfx.Factory
{
    [CreateAssetMenu(fileName = "Vfx Factory", menuName = "Factories/Vfx")]
    public class VfxFactory : GameObjectFactory, IVfxFactory
    {
        private const int DAMAGE_TEXT_WARMUP = 10;
        private const int MAX_DAMAGE_TEXT_POOL = 10;
        
        [Header("Unit VFX")]
        [SerializeField, Required] private UnitBlot _blotPrefab;
        [SerializeField, Required] private UnitExplosion _unitExplosionPrefab;
        [SerializeField, Required] private BaseSmoke _baseSmokePrefab;

        [Header("Skill VFX")]
        [SerializeField, Required] private IceDebuffView _iceDebuffPrefab;
        [SerializeField, Required] private KaboomView _kaboomPrefab;
        [SerializeField, Required] private HornView _hornPrefab;

        [Header("Other VFX")]
        [SerializeField, Required] private Shadow _shadowPrefab;
        [SerializeField, Required] private Wind _windPrefab;
        [SerializeField, Required] private FoodRecover _foodRecoverPrefab;

        [Header("Loot VFX")]
        [SerializeField, Required] private BattlePassLootPoint _battlePassLootPrefab;

        [Header("DamageText")]
        [SerializeField, Required] private DamageText _damageTextPrefab;

        private IAudioService _audioService;
        private ITargetRegistry _targetRegistry;
        private IWorldCameraService _cameraService;
        private IMyLogger _logger;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<Type, VfxEntity> _vfxLookup = new();
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<Type, Queue<VfxEntity>> _pools = new();
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<string, Queue<VfxEntity>> _keyedPools = new();
        [ShowInInspector, ReadOnly]
        private readonly Queue<DamageText> _damageTextPool = new();
        
        private int _damageTextCreated;

        public void Initialize(
            ITargetRegistry targetRegistry,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger)
        {
            _targetRegistry = targetRegistry;
            _cameraService = cameraService;
            _audioService = audioService;
            _logger = logger;
            
            _vfxLookup.Clear();
            _vfxLookup[typeof(UnitBlot)] = _blotPrefab;
            _vfxLookup[typeof(UnitExplosion)] = _unitExplosionPrefab;
            _vfxLookup[typeof(BaseSmoke)] = _baseSmokePrefab;
            _vfxLookup[typeof(IceDebuffView)] = _iceDebuffPrefab;
            _vfxLookup[typeof(KaboomView)] = _kaboomPrefab;
            _vfxLookup[typeof(HornView)] = _hornPrefab;
            _vfxLookup[typeof(Shadow)] = _shadowPrefab;
            _vfxLookup[typeof(Wind)] = _windPrefab;
            _vfxLookup[typeof(FoodRecover)] = _foodRecoverPrefab;
            _vfxLookup[typeof(BattlePassLootPoint)] = _battlePassLootPrefab;
        }

        public void Warmup()
        {
            WarmupDamageTextPool();
        }

        #region DamageText Pool
        private void WarmupDamageTextPool()
        {
            for (int i = 0; i < DAMAGE_TEXT_WARMUP; i++)
            {
                var instance = CreateDamageText();
                ReclaimDamageText(instance);
            }
        }
        
        public DamageText GetDamageText()
        {
            if (_damageTextPool.Count > 0)
            {
                var instance = _damageTextPool.Dequeue();
                instance.gameObject.SetActive(true);
                return instance;
            }

            if (_damageTextCreated < MAX_DAMAGE_TEXT_POOL)
            {
                var instance = CreateDamageText();
                _damageTextCreated++;
                return instance;
            }

            _logger.Log("DamageText pool reached max capacity!", DebugStatus.Warning);
            return null;
        }

        public void ReclaimDamageText(DamageText instance)
        {
            if (_damageTextPool.Count >= MAX_DAMAGE_TEXT_POOL)
                Destroy(instance.gameObject);
            else
            {
                instance.gameObject.SetActive(false);
                _damageTextPool.Enqueue(instance);
            }
        }

        private DamageText CreateDamageText()
        {
            var instance = CreateGameObjectInstance(_damageTextPrefab);
            instance.OriginFactory = this;
            instance.Construct(_cameraService, _targetRegistry, _audioService);
            return instance;
        }

        #endregion
        

        public T Get<T>() where T : VfxEntity
        {
            return GetInternal<T>();
        }

        public T Get<T>(Transform parent) where T : VfxEntity
        {
            return GetInternal<T>(parent);
        }
        
        private T GetInternal<T>(Transform parent = null) where T : VfxEntity
        {
            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var pool))
                _pools[type] = pool = new Queue<VfxEntity>();

            if (pool.Count > 0)
            {
                var instance = (T)pool.Dequeue();
                instance.gameObject.SetActive(true);
                if (parent != null)
                    instance.transform.SetParent(parent, worldPositionStays: true);
                return instance;
            }

            return Create<T>(parent);
        }
        
        private T Create<T>(Transform parent = null) where T : VfxEntity
        {
            if (!_vfxLookup.TryGetValue(typeof(T), out var prefab))
                return null;

            T instance;
            if (parent != null)
                instance = CreateGameObjectInstance(prefab, parent) as T;
            else
                instance = CreateGameObjectInstance(prefab) as T;

            if (instance != null)
            {
                instance.OriginFactory = this;
                
                instance.Construct(
                    _cameraService,
                    _targetRegistry,
                    _audioService);
            }
            
            return instance;
        }
        
        
        public async UniTask<T> GetAsync<T>(string key) where T : KeyedVfxEntity
        {
            if (!_keyedPools.TryGetValue(key, out var pool))
                _keyedPools[key] = pool = new Queue<VfxEntity>();

            T instance;

            if (pool.Count > 0)
            {
                instance = (T)pool.Dequeue();
                instance.gameObject.SetActive(true);
            }
            else
            {
                await UniTask.DelayFrame(1);
                instance = await CreateGameObjectInstanceAsync<T>(key);
                
                instance.OriginFactory = this;
                instance.Construct(_cameraService, _targetRegistry, _audioService);
            }

            instance.Key = key;

            return instance;
        }
        
        public void Reclaim<T>(T instance) where T : VfxEntity
        {
            instance.gameObject.SetActive(false);
            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var pool))
                _pools[type] = pool = new Queue<VfxEntity>();

            pool.Enqueue(instance);
        }
        
        public void Reclaim<T>(string key, T instance) where T : KeyedVfxEntity
        {
            if (instance == null || instance.OrNull() == null) return;

            instance.gameObject.SetActive(false);

            if (!_keyedPools.TryGetValue(key, out var pool))
                _keyedPools[key] = pool = new Queue<VfxEntity>();

            pool.Enqueue(instance);
        }
        
        public override void Cleanup()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj != null)
                        Destroy(obj.gameObject);
                }
            }
            
            _pools.Clear();
            
            foreach (var pool in _keyedPools.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj != null)
                        Destroy(obj.gameObject);
                }
            }
            
            _keyedPools.Clear();
            
            while (_damageTextPool.Count > 0)
            {
                var dt = _damageTextPool.Dequeue();
                if (dt != null)
                    Destroy(dt.gameObject);
            }

            _damageTextCreated = 0;

            base.Cleanup();
        }
    }
}