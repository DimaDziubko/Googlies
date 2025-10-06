using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Services._Camera;

namespace _Game.Core.GameState
{
    public class BattleLoadingState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;

        public BattleLoadingState(
            IGameStateMachine stateMachine,
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _logger = logger;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new BattleModeLoadingOperation(
                    _sceneLoader, 
                    _cameraService,
                    _logger));

            SimpleLoadingData data = 
                new SimpleLoadingData(LoadingScreenType.Simple, loadingOperations);
            
            _stateMachine.Enter<MenuState, SimpleLoadingData>(data);
        }

        public void Exit()
        {

        }
    }
}