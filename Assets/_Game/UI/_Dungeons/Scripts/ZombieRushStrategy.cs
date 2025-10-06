using System.Collections.Generic;
using _Game._BattleModes.Scripts;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Navigation.Timeline;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Game.UI._Dungeons.Scripts
{
    public class ZombieRushStrategy : IDungeonStrategy
    {
        private IGameStateMachine _state;
        private IDungeonModel _model;
        private readonly LoadingOperationFactory _loadingOperationFactory;
        private readonly IMainMenuProvider _mainMenuProvider;

        public ZombieRushStrategy(
            IGameStateMachine state,
            IDungeonModel model,
            LoadingOperationFactory loadingOperationFactory,
            IMainMenuProvider mainMenuProvider)
        {
            _state = state;
            _model = model;
            _loadingOperationFactory= loadingOperationFactory;
            _mainMenuProvider = mainMenuProvider;
        }
        
        public void Execute()
        {
            SimpleLoadingData simpleLoadingData =
                new SimpleLoadingData(LoadingScreenType.DarkFade, new Queue<ILoadingOperation>(2));
            
            var scene = SceneManager.GetSceneByName(Constants.Scenes.BATTLE_MODE);
            var sceneContext = scene.GetRoot<SceneContext>();
            var zombieRushMode = sceneContext.Container.Resolve<BattleMode>();

            var clearingModeOperation = _loadingOperationFactory.CreateClearModeOperation(zombieRushMode);
            var environmentClearingOperation = _loadingOperationFactory.CreateEnvironmentClearingOperation();
            var unitIconLoadingOperation = _loadingOperationFactory.CreateAgeWarriorIconsLoadingOperation(_model.TimelineId, 0);
            var menuReleasingOperation  = new MainMenuHideOperation(_mainMenuProvider);
            var battleModeUnloadingOperation = _loadingOperationFactory.CreateUnloadGameModeOperation(Constants.Scenes.BATTLE_MODE);
            var ambienceLoadingOperation = _loadingOperationFactory.CreateAmbienceLoadingOperation(new List<string>{_model.AmbienceKey});
            var zombieRushModeLoadingOperation = _loadingOperationFactory.CreateZombieRushModeLoadingOperation(_model);

            Queue<ILoadingOperation> parallelOperations = new Queue<ILoadingOperation>();
            
            parallelOperations.Enqueue(clearingModeOperation);
            parallelOperations.Enqueue(environmentClearingOperation);
            parallelOperations.Enqueue(unitIconLoadingOperation);
            parallelOperations.Enqueue(menuReleasingOperation);
            parallelOperations.Enqueue(battleModeUnloadingOperation);
            parallelOperations.Enqueue(ambienceLoadingOperation);

            simpleLoadingData.Operations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Loading zombie rush resources",parallelOperations));
            
            simpleLoadingData.Operations.Enqueue(zombieRushModeLoadingOperation);

            _state.Enter<DungeonLoopState, SimpleLoadingData>(simpleLoadingData);
        }
    }
    
}