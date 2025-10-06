using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Factory;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI._UIParticles;
using _Game.Utils.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._Coins.Factory
{
    public enum CurrencyVfxRenderMode
    {
        Camera,
        Overlay,
    }
    
    [CreateAssetMenu(fileName = "Coin Factory", menuName = "Factories/Coins")]
    public class CoinFactory : GameObjectFactory, ICoinFactory
    {
        private const int MAX_LOOT_COINS = 15;
        
        [SerializeField, Required] private LootFlyingReward _lootFlyingRewardPrefab;

        [SerializeField, Required] private FlyingCurrencyNew _coinsVfxPrefab;
        [SerializeField, Required] private FlyingCurrencyNew _gemsVfxPrefab;
        [SerializeField, Required] private FlyingCurrencyNew _skillPotionsVfxPrefab;
        
        [SerializeField, Required] private OverlayAttractorParent _overlayAttractorParentPrefab;
        
        [SerializeField, Required] private FlyingCurrencyParent _parent;
        
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<CurrencyType, Queue<FlyingCurrencyNew>> _currencyVfxCameraRenderPools = new();
        
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<CurrencyType, Queue<FlyingCurrencyNew>> _currencyVfxOverlayRenderPools = new();

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<CurrencyType, FlyingCurrencyNew> _currencyPrefabs = new();
        
        [ShowInInspector, ReadOnly]
        private readonly Queue<LootFlyingReward> _lootCoinsPool = new();

        private FlyingCurrencyParent _parentInstanceCamera;
        private FlyingCurrencyParent _parentInstanceOverlay;
        private OverlayAttractorParent _overlayAttractorParent;
        
        private IAudioService _audioService;
        private IWorldCameraService _cameraService;
        private IParticleAttractorRegistry _registry;
        private IMyLogger _logger;

        private int _lootCoinsCreated;
        
        public void Construct(
            IAudioService audioService,
            IWorldCameraService worldCameraService,
            IParticleAttractorRegistry registry,
            IMyLogger logger)
        {
            _logger = logger;
            _audioService = audioService;
            _cameraService = worldCameraService;
            
            _currencyPrefabs[CurrencyType.Coins] = _coinsVfxPrefab;
            _currencyPrefabs[CurrencyType.Gems] = _gemsVfxPrefab;
            _currencyPrefabs[CurrencyType.SkillPotion] = _skillPotionsVfxPrefab;

            foreach (CurrencyType type in _currencyPrefabs.Keys)
            {
                if (!_currencyVfxCameraRenderPools.ContainsKey(type))
                    _currencyVfxCameraRenderPools[type] = new Queue<FlyingCurrencyNew>();
            }
            
            foreach (CurrencyType type in _currencyPrefabs.Keys)
            {
                if (!_currencyVfxOverlayRenderPools.ContainsKey(type))
                    _currencyVfxOverlayRenderPools[type] = new Queue<FlyingCurrencyNew>();
            }
            
            _registry = registry;
            
            Cleanup();
        }

        public void Warmup()
        {
            WarmupLootCoins();
        }

        private void WarmupLootCoins()
        {
            for (int i = _lootCoinsPool.Count; i < MAX_LOOT_COINS; i++)
            {
                var lootCoin = CreateNewLootCoin();
                if (lootCoin != null)
                {
                    lootCoin.gameObject.SetActive(false);
                    _lootCoinsPool.Enqueue(lootCoin);
                }
            }
        }

        private FlyingCurrencyParent GetCameraRenderParent()
        {
            FlyingCurrencyParent instance = CreateGameObjectInstance(_parent);
            instance.OriginFactory = this;
            instance.Construct(_cameraService.UICameraOverlay);
            return instance;
        }
        
        public LootFlyingReward GetLootCoin()
        {
            if (_lootCoinsPool.Count > 0)
            {
                var lootCoin = _lootCoinsPool.Dequeue();
                lootCoin.gameObject.SetActive(true);
                return lootCoin;
            }
    
            if (_lootCoinsCreated < MAX_LOOT_COINS)
            {
                var newLoot = CreateNewLootCoin();
                if (newLoot != null)
                {
                    _lootCoinsCreated++;
                    newLoot.gameObject.SetActive(true);
                    return newLoot;
                }
            }
    
            return null;
        }

        public FlyingCurrencyNew GetCurrencyVfx(CurrencyType type, CurrencyVfxRenderMode currencyVfxRenderMode)
        {
            switch (currencyVfxRenderMode)
            {
                case CurrencyVfxRenderMode.Overlay:
                   return HandleOverlayRenderMode(type);
                default:
                    return HandleCameraRenderMode(type);
            }
        }

        private FlyingCurrencyNew HandleOverlayRenderMode(CurrencyType type)
        {
            _logger.Log("HANDLE OVERLAY RENDER MODE", DebugStatus.Info);
            
            if (_parentInstanceOverlay.OrNull() == null) _parentInstanceOverlay = GetOverlayRenderParent();
            if (_overlayAttractorParent.OrNull() == null)
            {
                _overlayAttractorParent = GetOverlayAttractorParent();
                _registry.Register(_overlayAttractorParent.CoinsAttractor.ParticleAttractableType, _overlayAttractorParent.CoinsAttractor.Attractor);
                _registry.Register(_overlayAttractorParent.GemsAttractor.ParticleAttractableType, _overlayAttractorParent.GemsAttractor.Attractor);
                _registry.Register(_overlayAttractorParent.SkillPotionAttractor.ParticleAttractableType, _overlayAttractorParent.SkillPotionAttractor.Attractor);
                
                _overlayAttractorParent.CoinsAttractor.Attractor.onAttracted.AddListener(OnCoinsAttracted);
                _overlayAttractorParent.GemsAttractor.Attractor.onAttracted.AddListener(OnGemsAttracted);
                _overlayAttractorParent.SkillPotionAttractor.Attractor.onAttracted.AddListener(OnSkillPotionsAttracted);
            }
            
            if (!_currencyPrefabs.ContainsKey(type))
            {
                return null;
            }
            
            if (!_currencyVfxOverlayRenderPools.TryGetValue(type, out var pool))
            {
                pool = new Queue<FlyingCurrencyNew>();
                _currencyVfxOverlayRenderPools[type] = pool;
            }
            
            if (_currencyVfxOverlayRenderPools[type].Count > 0)
            {
                FlyingCurrencyNew pooled = _currencyVfxOverlayRenderPools[type].Dequeue();
                pooled.SetActive(true);
                return pooled;
            }
            
            if (_registry.TryGetAttractor(type.ToAttractableVfxRenderedOverlay(), out var attractor))
            {
                FlyingCurrencyNew instance = CreateGameObjectInstance(_currencyPrefabs[type], _parentInstanceOverlay.Transform);
                instance.OriginFactory = this;
                instance.RenderMode = CurrencyVfxRenderMode.Overlay;
                instance.Construct(attractor);
                return instance;
            }
            
            return null;
        }

        private void OnSkillPotionsAttracted() => 
            _audioService.PlayVfxAttractSound(AttractableParticleType.SkillPotions);

        private void OnGemsAttracted() => 
            _audioService.PlayVfxAttractSound(AttractableParticleType.GemsCamera);

        private void OnCoinsAttracted() => 
            _audioService.PlayVfxAttractSound(AttractableParticleType.CoinsCamera);

        private OverlayAttractorParent GetOverlayAttractorParent()
        {
            OverlayAttractorParent instance = CreateGameObjectInstance(_overlayAttractorParentPrefab);
            instance.OriginFactory = this;
            return instance;
        }

        private FlyingCurrencyParent GetOverlayRenderParent()
        {
            FlyingCurrencyParent instance = CreateGameObjectInstance(_parent);
            instance.OriginFactory = this;
            return instance;
        }

        private FlyingCurrencyNew HandleCameraRenderMode(CurrencyType type)
        {
            if (_parentInstanceCamera.OrNull() == null) _parentInstanceCamera = GetCameraRenderParent();
            
            if (!_currencyPrefabs.ContainsKey(type))
            {
                return null;
            }

            if (!_currencyVfxCameraRenderPools.TryGetValue(type, out var pool))
            {
                pool = new Queue<FlyingCurrencyNew>();
                _currencyVfxCameraRenderPools[type] = pool;
            }
            
            if (_currencyVfxCameraRenderPools[type].Count > 0)
            {
                FlyingCurrencyNew pooled = _currencyVfxCameraRenderPools[type].Dequeue();
                pooled.SetActive(true);
                return pooled;
            }

            if (_registry.TryGetAttractor(type.ToAttractableVfxRenderedCamera(), out var attractor))
            {
                FlyingCurrencyNew instance = CreateGameObjectInstance(_currencyPrefabs[type], _parentInstanceCamera.Transform);
                instance.OriginFactory = this;
                instance.RenderMode = CurrencyVfxRenderMode.Camera;
                instance.Construct(attractor);
                return instance;
            }
            
            return null;
        }
        
        
        private LootFlyingReward CreateNewLootCoin()
        {
            var newLootCoin = CreateGameObjectInstance(_lootFlyingRewardPrefab);
            if (newLootCoin != null)
                newLootCoin.OriginFactory = this;

            return newLootCoin;
        }

        public void Reclaim(FlyingReward flyingReward)
        {
            switch (flyingReward)
            {
                case LootFlyingReward lootCoin:
                    lootCoin.gameObject.SetActive(false);
                    _lootCoinsPool.Enqueue(lootCoin);
                    break;
                
                case FlyingCurrencyNew currencyVfx:
                    currencyVfx.gameObject.SetActive(false);
                    HandleCurrencyReclaim(currencyVfx, currencyVfx.RenderMode);
                    break;
                
                default:
                    Destroy(flyingReward.gameObject);
                    break;
            }
        }

        private void HandleCurrencyReclaim(FlyingCurrencyNew currencyVfx, CurrencyVfxRenderMode renderMode)
        {
            CurrencyType type = currencyVfx.CurrencyType;
            if (renderMode == CurrencyVfxRenderMode.Overlay)
            {
                
                if (!_currencyVfxOverlayRenderPools.ContainsKey(type))
                    _currencyVfxOverlayRenderPools[type] = new Queue<FlyingCurrencyNew>();
                _currencyVfxOverlayRenderPools[type].Enqueue(currencyVfx);
                return;
            }
            
            if (!_currencyVfxCameraRenderPools.ContainsKey(type))
                _currencyVfxCameraRenderPools[type] = new Queue<FlyingCurrencyNew>();
            _currencyVfxCameraRenderPools[type].Enqueue(currencyVfx);
        }

        public override void Cleanup()
        {
            while (_lootCoinsPool.Count > 0)
            {
                Destroy(_lootCoinsPool.Dequeue().gameObject);
            }

            _lootCoinsPool.Clear();
            _lootCoinsCreated = 0;
            
            foreach (var queue in _currencyVfxCameraRenderPools.Values)
            {
                while (queue.Count > 0)
                    Destroy(queue.Dequeue().gameObject);
            }
            
            _currencyVfxCameraRenderPools.Clear();
            
            foreach (var queue in _currencyVfxOverlayRenderPools.Values)
            {
                while (queue.Count > 0)
                    Destroy(queue.Dequeue().gameObject);
            }
            
            _currencyVfxOverlayRenderPools.Clear();
            
            
            if (_parentInstanceCamera.OrNull() != null)
            {
                Destroy(_parentInstanceCamera.gameObject);
                _parentInstanceCamera = null;
            }
            
            if (_parentInstanceOverlay.OrNull() != null)
            {
                Destroy(_parentInstanceOverlay.gameObject);
                _parentInstanceOverlay = null;
            }
            
            if (_overlayAttractorParent.OrNull() != null)
            {
                _overlayAttractorParent.CoinsAttractor.Attractor.onAttracted.RemoveListener(OnCoinsAttracted);
                _overlayAttractorParent.GemsAttractor.Attractor.onAttracted.RemoveListener(OnGemsAttracted);
                _overlayAttractorParent.SkillPotionAttractor.Attractor.onAttracted.RemoveListener(OnSkillPotionsAttracted);
                
                _registry.TryDeregister(_overlayAttractorParent.CoinsAttractor.ParticleAttractableType);
                _registry.TryDeregister(_overlayAttractorParent.GemsAttractor.ParticleAttractableType);
                _registry.TryDeregister(_overlayAttractorParent.SkillPotionAttractor.ParticleAttractableType);
                
                Destroy(_overlayAttractorParent.gameObject);
                _overlayAttractorParent = null;
            }
        }
    }
}