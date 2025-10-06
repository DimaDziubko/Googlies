using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Core.Factory;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.Analytics;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._Environment;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using DevToDev.Analytics;

namespace _Game._BattleModes.Scripts
{
    public class ZombieRushMode :
        IGameModeCleaner,
        IBaseDestructionStartHandler,
        IBaseDestructionCompleteHandler,
        IAllEnemiesDefeatedHandler,
        IEndGameListener
    {
        public IEnumerable<GameObjectFactory> Factories => _factoryHolder.Factories;
        private readonly IFactoriesHolder _factoryHolder;
        public string SceneName => Constants.Scenes.ZOMBIE_RUSH_MODE;

        private readonly IBattleTriggersManager _battleTriggersManager;

        private readonly IGameStateMachine _state;
        private readonly IGameManager _gameManager;
        private readonly IMyLogger _logger;
        private readonly ZombieRushBattle _battle;
        private readonly DungeonEnvironmentController _environmentController;
        private readonly DungeonResultPopupController _popupController;
        private readonly LoadingOperationFactory _loadingOperationFactory;
        private readonly IAnalyticsEventSender _sender;

        private IDungeonModel _dungeonModel;

        private int _loadedTimelineId;

        private bool _isBattleEnded;
        
        public ZombieRushMode(
            IGameStateMachine state,
            IBattleTriggersManager battleTriggersManager,
            IFactoriesHolder factoriesHolder,
            IGameManager gameManager,
            IMyLogger logger,
            ZombieRushBattle battle,
            DungeonEnvironmentController environmentController,
            DungeonResultPopupController popupController,
            LoadingOperationFactory loadingOperationFactory,
            IAnalyticsEventSender sender
        )
        {
            _state = state;
            _factoryHolder = factoriesHolder;
            _logger = logger;
            _battleTriggersManager = battleTriggersManager;
            _gameManager = gameManager;
            _environmentController = environmentController;
            _battle = battle;
            _popupController = popupController;
            _loadingOperationFactory = loadingOperationFactory;
            _sender = sender;
        }

        public async UniTask Initialize(IDungeonModel dungeonModel, ScenarioState scenarioState)
        {
            _dungeonModel = dungeonModel;
            _loadedTimelineId = _dungeonModel.TimelineId;

            await _environmentController.Initialize(dungeonModel.EnvironmentKey);
            _battleTriggersManager.Register(this);
            await _battle.Initialize(scenarioState, dungeonModel.AmbienceKey);
        }

        public void BeginNewGame()
        {
            _gameManager.StartBattle();

            SendStart();
        }

        private void SendStart()
        {
            //DTD
            var parameters = new DTDCustomEventParameters();

            var levelKey = $"{_dungeonModel.DungeonType}_Level";
            var stageKey = $"{_dungeonModel.DungeonType}_Timeline";
            var subLevelKey = $"{_dungeonModel.DungeonType}_Battle";

             parameters.Add(levelKey, _dungeonModel.CurrentLevel);
             parameters.Add(stageKey, _dungeonModel.Stage);
             parameters.Add(subLevelKey, _dungeonModel.SubLevel);

             _sender.CustomEvent("dungeon_battle_started", parameters);

            _logger.Log($"dungeon_battle_started {_dungeonModel.DungeonType}_Level{_dungeonModel.CurrentLevel} {_dungeonModel.DungeonType}_Timeline_{_dungeonModel.Stage} {_dungeonModel.DungeonType}_Battle_{_dungeonModel.SubLevel}", DebugStatus.Info);
        }

        void IGameModeCleaner.Cleanup()
        {
            ResetGame();
            _battle.Cleanup();
        }

        private void ResetGame() => _battle.Reset();

        void IBaseDestructionStartHandler.OnBaseDestructionStarted(Faction faction, Base @base)
        {
            _gameManager.StopBattle();
            _gameManager.SetPaused(true);
        }

        void IBaseDestructionCompleteHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            switch (faction)
            {
                case Faction.Player:
                    _gameManager.EndBattle(GameResultType.Defeat);
                    break;
                case Faction.Enemy:
                    _gameManager.EndBattle(GameResultType.Victory);
                    break;
            }
        }
        void IAllEnemiesDefeatedHandler.OnAllUnitsDefeated()
        {
            if(_isBattleEnded) return;
            _gameManager.SetPaused(true);
            _gameManager.StopBattle();
            _gameManager.EndBattle(GameResultType.Victory);
        }

        async void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit)
        {
            _isBattleEnded = true;
            
            SendComplete(result, wasExit);

            if (result == GameResultType.Victory && !wasExit)
            {
                await _popupController.ShowPopup(_dungeonModel);
            }
            
            GoToMainMenu();
        }

        private void SendComplete(GameResultType result, bool wasExit)
        {
            //DTD

            var parameters = new DTDCustomEventParameters();

            var resultKey = "Result";
            string resultValue = "Unknown";

            if (result == GameResultType.Victory)
            {
                resultValue = "Victory";
            }
            else if (result == GameResultType.Defeat && !wasExit)
            {
                resultValue = "Defeat";
            }
            else if (result == GameResultType.Defeat && wasExit)
            {
                resultValue = "Exit";
            }

            var levelKey = $"{_dungeonModel.DungeonType}_Level";
            var stageKey = $"{_dungeonModel.DungeonType}_Timeline";
            var subLevelKey = $"{_dungeonModel.DungeonType}_Battle";

            parameters.Add(resultKey, resultValue);
            parameters.Add(levelKey, _dungeonModel.CurrentLevel);
            parameters.Add(stageKey, _dungeonModel.Stage);
            parameters.Add(subLevelKey, _dungeonModel.SubLevel);

            _sender.CustomEvent("dungeon_battle_completed", parameters);
            _logger.Log($"dungeon_battle_completed {_dungeonModel.DungeonType}_Level{_dungeonModel.CurrentLevel} {_dungeonModel.DungeonType}_Timeline_{_dungeonModel.Stage} {_dungeonModel.DungeonType}_Battle_{_dungeonModel.SubLevel}", DebugStatus.Info);
        }

        private void GoToMainMenu()
        {
            Queue<ILoadingOperation> loadingOperations = new Queue<ILoadingOperation>();
            Queue<ILoadingOperation> parallelOperations1 = new Queue<ILoadingOperation>();
            Queue<ILoadingOperation> parallelOperations2 = new Queue<ILoadingOperation>();
            
            parallelOperations1.Enqueue(_loadingOperationFactory.CreateEnvironmentClearingOperation());
            parallelOperations1.Enqueue(_loadingOperationFactory.CreateClearModeOperation(this));
            parallelOperations1.Enqueue(_loadingOperationFactory.CreateAgeWarriorIconsReleasingOperation(_loadedTimelineId, 0));
            parallelOperations1.Enqueue(_loadingOperationFactory.CreateAmbienceReleasingOperation(new List<string> { _dungeonModel.AmbienceKey }));

            loadingOperations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Releasing zombie rush resources", parallelOperations1));
            
            parallelOperations2.Enqueue(_loadingOperationFactory.CreateUnloadGameModeOperation(Constants.Scenes.ZOMBIE_RUSH_MODE));
            parallelOperations2.Enqueue(_loadingOperationFactory.CreateBattleModeLoadingOperation());
            
            loadingOperations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Change scene", parallelOperations2));

            GoToMainMenu(loadingOperations);
        }

        private void GoToMainMenu(Queue<ILoadingOperation> operations)
        {
            SimpleLoadingData data = new SimpleLoadingData(LoadingScreenType.DarkFade, operations);
            _state.Enter<MenuState, SimpleLoadingData>(data);
        }
    }
}