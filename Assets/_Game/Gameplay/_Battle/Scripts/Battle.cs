using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Core.CustomKernel;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Scenario;
using _Game.UI._Environment;
using _Game.UI._Hud;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._Battle.Scripts
{
    public class Battle :
        IGameTickable,
        IGameLateTickable,
        IPauseListener,
        IStartGameListener,
        IStopGameListener
    //IBattleSpeedListener
    {
        private readonly BattleField _battleField;

        private readonly AmbienceController _ambienceController;
        private readonly BattleEnvironmentController _environmentController;
        private readonly IUserContainer _userContainer;

        private BattleScenarioExecutor _scenarioExecutor;
        private BattleScenarioExecutor.State _activeScenario;
        private int _currentCachedWave;

        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        private readonly BattleHud _battleHud;

        private float _speedFactor;
        private bool _isPaused;
        private bool _isRunning;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public Battle(
            AmbienceController ambienceController,
            BattleField battleField,
            BattleHud battleHud,
            BattleEnvironmentController environmentController,
            IUserContainer userContainer,
            IMyLogger logger
            )
        {
            _battleField = battleField;
            _ambienceController = ambienceController;
            _battleHud = battleHud;
            _environmentController = environmentController;
            _userContainer = userContainer;
            _logger = logger;

            _scenarioExecutor = new BattleScenarioExecutor();
        }

        public void Initialize() => _battleField.Init();

        public async UniTask OnBattleChanged(BattleConfig battleConfig)
        {
            _logger.Log("BATTLE ON BATTLE CHANGED", DebugStatus.Warning);

            _ambienceController.SetAmbience(battleConfig.AmbienceKey);
            _scenarioExecutor.UpdateScenario(battleConfig.Scenario);
            await _environmentController.ShowEnvironment(battleConfig.EnvironmentKey);
            await _battleField.OnBattleChanged();
        }

        public async UniTask OnAgeChanged()
        {
            _logger.Log("BATTLE ON AGE CHANGED", DebugStatus.Warning);
            await _battleField.OnAgeChanged();
        }

        void IStartGameListener.OnStartBattle()
        {
            _battleField.EnemyUnitSpawner.UnitSpawned += OnEnemyUnitSpawned;

            AstarPath.active.Scan();

            _activeScenario = _scenarioExecutor.Begin(_battleField);
            _battleField.StartBattle();

            _ambienceController.PlayAmbience();
            _currentCachedWave = 0;
            _isRunning = true;
        }

        void IStopGameListener.OnStopBattle()
        {
            _isRunning = false;
            _battleField.EnemyUnitSpawner.UnitSpawned -= OnEnemyUnitSpawned;
            _ambienceController.StopAmbience();
        }

        public async UniTask Reset()
        {
            _logger.Log("BATTLE RESET", DebugStatus.Warning);
            await _battleField.UpdateBases();
        }

        void IGameTickable.Tick(float deltaTime)
        {
            if (!_isPaused && _isRunning)
                _activeScenario.Progress(deltaTime * 1);

            _battleField.GameUpdate(deltaTime);
        }

        void IGameLateTickable.LateTick(float deltaTime) =>
            _battleField.LateGameUpdate(deltaTime);

        void IPauseListener.SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
            _battleField.SetPaused(isPaused);
        }

        // void IBattleSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        // {
        //     _speedFactor = speedFactor;
        //     _battleField.SetSpeedFactor(speedFactor);
        // }

        private void OnEnemyUnitSpawned(UnitBase unitBase) => TryToShowWave();

        private void TryToShowWave()
        {
            var waves = _activeScenario.GetWaves();
            int currentWave = waves.currentWave;
            if (_currentCachedWave != currentWave && currentWave <= waves.wavesCount)
            {
                _battleHud.WaveInfoPopup.ShowWave(currentWave, waves.wavesCount);
                _currentCachedWave = currentWave;
                TimelineState.TrackWave(currentWave);
            }
        }

        public void Cleanup() =>
            _battleField.Cleanup();

        public void Dispose() =>
            _battleField.Dispose();

        public void OnTimelineChanged() =>
            _battleField.OnTimelineChanged();
    }
}