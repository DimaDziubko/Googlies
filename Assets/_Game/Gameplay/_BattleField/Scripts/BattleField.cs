using System;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Factory;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Hud;
using _Game.UI._Hud._CoinCounterView;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBattleField
    {
        public IUnitSpawner PlayerUnitSpawner { get; }
        public IUnitSpawner EnemyUnitSpawner { get; }
        public IPickUpProxy PickUpSpawner { get; }
        public IVFXProxy VFXProxy { get; }
        BattleFieldSettings Settings { get; }
    }

    public class BattleField :
        IBaseDestructionStartHandler,
        IBattlefieldPointsProvider,
        IUnitSpawnProxy,
        IBattleField
    {
        public IUnitSpawner PlayerUnitSpawner => _playerUnitSpawner;
        public IUnitSpawner EnemyUnitSpawner => _enemyUnitSpawner;
        public IPickUpProxy PickUpSpawner => _pickUpSpawner;
        public IVFXProxy VFXProxy => _vfxSpawner;
        public BattleFieldSettings Settings => _battleFieldSettings;

        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        [ShowInInspector, ReadOnly]
        private readonly LootCoinSpawner _lootCoinSpawner;
        [ShowInInspector, ReadOnly]
        private readonly VfxSpawner _vfxSpawner;
        [ShowInInspector, ReadOnly]
        private readonly UnitSpawner _playerUnitSpawner;
        [ShowInInspector, ReadOnly]
        private readonly UnitSpawner _enemyUnitSpawner;
        [ShowInInspector, ReadOnly]
        private readonly PickUpSpawner _pickUpSpawner;

        [ShowInInspector, ReadOnly]
        private readonly ProjectileSpawner _projectileSpawner;
        
        private readonly BaseSpawner _playerBaseSpawner;
        private readonly BaseSpawner _enemyBaseSpawner;

        private readonly CoinCounterView _coinCounterView;
        private readonly BattleFieldSettings _battleFieldSettings;

        private readonly IMyLogger _logger;
        
        private bool _isPaused;

        public Vector3 PlayerSpawnPoint => _battleFieldSettings.PlayerSpawnPoint;
        public Vector3 EnemySpawnPoint => _battleFieldSettings.EnemySpawnPoint;
        public Vector3 EnemyDestinationPoint => _battleFieldSettings.PlayerBasePoint;
        public Vector3 PlayerDestinationPoint => _battleFieldSettings.EnemyBasePoint;
        public Vector3 PlayerBaseSpawnPoint => _battleFieldSettings.PlayerBasePoint;
        public Vector3 EnemyBaseSpawnPoint => _battleFieldSettings.EnemyBasePoint;

        public BattleField(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IBattleTriggersManager battleTriggersManager,
            ICoinCounter coinCounter,
            IFactoriesHolder factoriesHolder,
            BattleHud battleHud,
            IBattleModeBaseDataProvider baseDataProvider,
            IBattleSpeedManager speedManager,
            BattleFieldSettings battleFieldSettings,
            IUserContainer userContainer,
            IMyLogger logger,
            IAdsService adsService
        )
        {
            _logger = logger;
            
            _cameraService = cameraService;
            _audioService = audioService;

            _lootCoinSpawner = new LootCoinSpawner(
                factoriesHolder.CoinFactory,
                audioService,
                coinCounter,
                logger);

            _vfxSpawner = new VfxSpawner(
                factoriesHolder.VfxFactory,
                userContainer.State.SettingsSaveGame,
                _logger);

            _projectileSpawner = new ProjectileSpawner(
                factoriesHolder.ProjectileFactory,
                _vfxSpawner,
                speedManager);

            IUnitEventHandler playerUnitEventHandler = new PlayerUnitEventHandler(_vfxSpawner);
            IUnitEventHandler enemyUnitEventHandler = new EnemyUnitEventHandler(_lootCoinSpawner, _vfxSpawner);

            _playerUnitSpawner = new UnitSpawner(
                factoriesHolder.UnitFactory,
                _vfxSpawner,
                _projectileSpawner,
                speedManager,
                playerUnitEventHandler,
                Faction.Player);

            _enemyUnitSpawner = new UnitSpawner(
                factoriesHolder.UnitFactory,
                _vfxSpawner,
                _projectileSpawner,
                speedManager,
                enemyUnitEventHandler,
                Faction.Enemy);

            _pickUpSpawner = new PickUpSpawner(factoriesHolder.PickUpFactory);

            IBaseEventHandler playerBaseEventHandler = new PlayerBaseEventHandler(battleTriggersManager, _vfxSpawner);
            IBaseEventHandler enemyBaseEventHandler =
                new EnemyBaseEventHandler(battleTriggersManager, _lootCoinSpawner, _vfxSpawner);

            _playerBaseSpawner = new BaseSpawner(
                factoriesHolder.BaseFactory,
                baseDataProvider,
                playerBaseEventHandler,
                logger,
                Faction.Player);

            _enemyBaseSpawner = new BaseSpawner(
                factoriesHolder.BaseFactory,
                baseDataProvider,
                enemyBaseEventHandler,
                logger,
                Faction.Enemy);

            _coinCounterView = battleHud.CounterView;

            _battleFieldSettings = battleFieldSettings;

            battleTriggersManager.Register(this);

            adsService.ShowBanner();
        }

        public void Init()
        {
            _battleFieldSettings.Initialize(_cameraService);
            
            _lootCoinSpawner.Init(_coinCounterView.CoinIconHolderPosition);

            var playerBaseRotation = Quaternion.Euler(0, 0, 0);
            var enemyBaseRotation = Quaternion.Euler(0, 180, 0);

            _playerBaseSpawner.Init(PlayerBaseSpawnPoint, playerBaseRotation);
            _enemyBaseSpawner.Init(EnemyBaseSpawnPoint, enemyBaseRotation);

            SetSpeedFactor(1);
        }

        public void StartBattle()
        {
            _playerBaseSpawner.OnStartBattle();
            _enemyBaseSpawner.OnStartBattle();
            _vfxSpawner.OnStartBattle();
            _lootCoinSpawner.OnStartBattle();
        }

        private void SetSpeedFactor(float speedFactor)
        {
            _projectileSpawner.SetSpeedFactor(speedFactor);
            _playerUnitSpawner.SetSpeedFactor(speedFactor);
            _enemyUnitSpawner.SetSpeedFactor(speedFactor);
        }

        public void GameUpdate(float deltaTime)
        {
            _vfxSpawner.GameUpdate(deltaTime);
            _pickUpSpawner.GameUpdate(deltaTime);
            _playerUnitSpawner.GameUpdate(deltaTime);
            _enemyUnitSpawner.GameUpdate(deltaTime);
            _projectileSpawner.GameUpdate(deltaTime);  
            _lootCoinSpawner.GameUpdate(deltaTime);
        }

        public void LateGameUpdate(float deltaTime)
        {
            _vfxSpawner.LateGameUpdate(deltaTime);
            _enemyUnitSpawner.LateGameUpdate(deltaTime);
            _playerUnitSpawner.LateGameUpdate(deltaTime);
        }

        public async UniTask UpdateBases()
        {
            _logger.Log("UPDATE BASES", DebugStatus.Warning);
            await _playerBaseSpawner.UpdateState();
            await _enemyBaseSpawner.UpdateState();
        }

        public void Cleanup()
        {
            _playerUnitSpawner.Cleanup();
            _enemyUnitSpawner.Cleanup();
            _projectileSpawner.Cleanup();
            _playerBaseSpawner.Cleanup();
            _enemyBaseSpawner.Cleanup();
            _pickUpSpawner.Cleanup();
            _vfxSpawner.Cleanup();
            _lootCoinSpawner.Cleanup();
        }

        void IBaseDestructionStartHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            _enemyUnitSpawner.SetPaused(true);
            _projectileSpawner.SetPaused(true);
            _playerUnitSpawner.SetPaused(true);
            
            _vfxSpawner.SpawnBasesSmoke(@base.Position);
            _audioService.PlayBaseDestructionSFX();
            _playerUnitSpawner.ResetUnits();

            switch (faction)
            {
                case Faction.Player:
                    _playerUnitSpawner.KillUnits();
                    break;
                case Faction.Enemy:
                    _enemyUnitSpawner.KillUnits();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
            _playerUnitSpawner.SetPaused(isPaused);
            _enemyUnitSpawner.SetPaused(isPaused);
            _projectileSpawner.SetPaused(isPaused);
            _playerBaseSpawner.SetPaused(isPaused);
            _enemyBaseSpawner.SetPaused(isPaused);
        }

        async UniTask IUnitSpawnProxy.SpawnPlayerUnit(UnitType type, Skin skin)
        {
            UnitBase unit = await PlayerUnitSpawner.SpawnUnit(type, skin);
            unit.Position = PlayerSpawnPoint;
            unit.Destination = EnemyBaseSpawnPoint;
            unit.Rotation = Quaternion.Euler(0, 0, 0);
            unit.Pusher.Initialize(_battleFieldSettings.PlayerBaseBound, true, true);
        }

        async UniTask IUnitSpawnProxy.SpawnEnemyUnit(UnitType type, Skin skin)
        {
            UnitBase unit = await EnemyUnitSpawner.SpawnUnit(type, skin);
            unit.Position = EnemySpawnPoint;
            unit.Destination = PlayerBaseSpawnPoint;
            unit.Rotation = Quaternion.Euler(0, 180, 0);
            unit.Pusher.Initialize(_battleFieldSettings.EnemyBaseBound, false, true);
        }
        
        public void OnTimelineChanged()
        {
            _logger.Log("BATTLEFIELD ON TIMELINE CHANGED", DebugStatus.Warning);
            _playerBaseSpawner.Cleanup();
            _enemyBaseSpawner.Cleanup();
        }

        public async UniTask OnBattleChanged()
        {
            _logger.Log("BATTLEFIELD ON BATTLE CHANGED", DebugStatus.Warning);
            await _enemyBaseSpawner.UpdateState();
        }

        public async UniTask OnAgeChanged()
        {
            _logger.Log("BATTLEFIELD ON AGE CHANGED", DebugStatus.Warning);
            await _playerBaseSpawner.UpdateState();
        }

        public void Dispose()
        {
            
        }
    }
}