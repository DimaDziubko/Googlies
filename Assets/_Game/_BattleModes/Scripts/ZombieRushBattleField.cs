using System;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Factory;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Hud;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._BattleModes.Scripts
{
    public class ZombieRushBattleField : 
        IBaseDestructionStartHandler, 
        IDisposable,
        IBattlefieldPointsProvider,
        IUnitSpawnProxy,
        IZombieDeathHandler,
        IBattleField
    {
        private readonly IAudioService _audioService;
        private readonly IBattleTriggersManager _battleTriggersManager;
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;

        private readonly ZombieRushHud _hud;

        private readonly UnitSpawner _playerUnitSpawner;
        private readonly UnitSpawner _enemyUnitSpawner;
        private readonly VfxSpawner _vfxSpawner;
        private readonly ProjectileSpawner _projectileSpawner;

        private readonly BaseSpawner _playerBaseSpawner;
        
        [ShowInInspector, ReadOnly]
        private readonly PickUpSpawner _pickUpSpawner;

        public IUnitSpawner PlayerUnitSpawner => _playerUnitSpawner;
        public IUnitSpawner EnemyUnitSpawner => _enemyUnitSpawner;

        public IPickUpProxy PickUpSpawner => _pickUpSpawner;

        public IVFXProxy VFXProxy => _vfxSpawner;

        public BattleFieldSettings Settings => _settings;

        private readonly IBaseEventHandler _playerBaseEventHandler;

        private readonly BattleFieldSettings _settings;

        private int _zombiesDefeated;
        private int _totalZombiesCount;
        private bool _isPaused;
        public Vector3 PlayerSpawnPoint => _settings.PlayerSpawnPoint;
        public Vector3 EnemySpawnPoint => _settings.EnemySpawnPoint;
        public Vector3 EnemyDestinationPoint => _settings.PlayerDestinationPoint;
        public Vector3 PlayerDestinationPoint => _settings.PlayerDestinationPoint;
        public Vector3 PlayerBaseSpawnPoint => _settings.PlayerBasePoint;
        public Vector3 EnemyBaseSpawnPoint => _settings.EnemyBasePoint;
        
        public ZombieRushBattleField(
            IFactoriesHolder factoriesHolder,
            IBattleSpeedManager speedManager,
            IWorldCameraService cameraService,
            IBattleTriggersManager battleTriggersManager,
            IAudioService audioService,
            IUserContainer userContainer,
            IZombieRushModeBaseDataProvider baseDataProvider,
            IBaseFactory baseFactory,
            ZombieRushHud hud,
            BattleFieldSettings settings, 
            IMyLogger logger)
        {
            _logger = logger;
            
            _pickUpSpawner = new PickUpSpawner(factoriesHolder.PickUpFactory);
            
            _vfxSpawner = new VfxSpawner(
                factoriesHolder.VfxFactory, 
                userContainer.State.SettingsSaveGame,
                logger);
            
            _playerBaseEventHandler = new PlayerBaseEventHandler(battleTriggersManager, _vfxSpawner);
            
            _projectileSpawner = new ProjectileSpawner(
                factoriesHolder.ProjectileFactory,
                _vfxSpawner,
                speedManager);
            
            IUnitEventHandler playerUnitEventHandler = new PlayerUnitEventHandler(_vfxSpawner);
            IUnitEventHandler ratsRushEnemyEventHandler = new ZombieRushEnemyEventHandler(_vfxSpawner, this);
            
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
                ratsRushEnemyEventHandler,
                Faction.Enemy);
            
            _playerBaseSpawner = new BaseSpawner(
                baseFactory,
                baseDataProvider,
                _playerBaseEventHandler,
                logger,
                Faction.Player);
            
            _hud = hud;
            _audioService = audioService;
            _battleTriggersManager = battleTriggersManager;
            _settings = settings;
            _cameraService = cameraService;
            
            battleTriggersManager.Register(this);
        }

        public void Initialize(int totalRatsCount)
        {
            _totalZombiesCount = totalRatsCount;
            _zombiesDefeated = 0;
            
            _settings.Initialize(_cameraService);

            var playerBaseRotation = Quaternion.Euler(0, 0, 0);
            
            _playerBaseSpawner.Init(PlayerBaseSpawnPoint, playerBaseRotation);
        }

        public void Dispose()
        {
            _battleTriggersManager.UnRegister(this);
        }

        public void GameUpdate(float deltaTime)
        {
            _vfxSpawner.GameUpdate(deltaTime);
            _playerUnitSpawner.GameUpdate(deltaTime);
            _enemyUnitSpawner.GameUpdate(deltaTime);
            _projectileSpawner.GameUpdate(deltaTime);
        }

        public void LateGameUpdate(float deltaTime)
        {
            _vfxSpawner.LateGameUpdate(deltaTime);
            _enemyUnitSpawner.LateGameUpdate(deltaTime);
            _playerUnitSpawner.LateGameUpdate(deltaTime);
        }

        public void OnStartBattle()
        {
            UpdateRatsCounterView();
            _playerBaseSpawner.OnStartBattle();
            _vfxSpawner.OnStartBattle();
        }

        private void UpdateRatsCounterView() =>
            _hud.UpdateRatsCounter($"Zombies defeated {_zombiesDefeated}/{_totalZombiesCount}");

        public void ResetSelf() => _zombiesDefeated = 0;

        public async UniTask UpdatePlayerBase() => await _playerBaseSpawner.UpdateState();

        void IBaseDestructionStartHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            _vfxSpawner.SpawnBasesSmoke(@base.Position);
            _audioService.PlayBaseDestructionSFX();
            _playerUnitSpawner.ResetUnits();
            _enemyUnitSpawner.ResetUnits();

            switch (faction)
            {
                case Faction.Player:
                    _playerUnitSpawner.KillUnits();
                    break;
                case Faction.Enemy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        public void Cleanup()
        {
            _playerUnitSpawner.Cleanup();
            _enemyUnitSpawner.Cleanup();
            _projectileSpawner.Cleanup();
            _vfxSpawner.Cleanup();
        }

        async UniTask IUnitSpawnProxy.SpawnPlayerUnit(UnitType type, Skin skin)
        {
            var unit = await PlayerUnitSpawner.SpawnUnit(type, skin);
            unit.Position = PlayerSpawnPoint;
            unit.Destination = PlayerDestinationPoint;
            unit.Rotation = Quaternion.Euler(0, 0, 0);
            unit.Pusher.Initialize(_settings.PlayerBaseBound, true, true);
        }

        async UniTask IUnitSpawnProxy.SpawnEnemyUnit(UnitType type, Skin skin)
        {
            var unit = await EnemyUnitSpawner.SpawnUnit(type, skin);
            unit.Position = EnemySpawnPoint;
            unit.Destination = PlayerBaseSpawnPoint;
            unit.Rotation = Quaternion.Euler(0, 180, 0);
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
            _playerUnitSpawner.SetPaused(isPaused);
            _enemyUnitSpawner.SetPaused(isPaused);
            _projectileSpawner.SetPaused(isPaused);
            _playerBaseSpawner.SetPaused(isPaused);
        }

        void IZombieDeathHandler.OnZombieDead()
        {
            _zombiesDefeated++;
            UpdateRatsCounterView();

            if (_zombiesDefeated == _totalZombiesCount) 
                _battleTriggersManager.AllEnemiesDefeated();
        }
    }
}