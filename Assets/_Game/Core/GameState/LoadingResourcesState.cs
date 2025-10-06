using System.Collections.Generic;
using _Game._BattleModes.Scripts;
using _Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class LoadingResourcesState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly LoadingOperationFactory _loadingOperationFactory;

        public LoadingResourcesState(
            IGameStateMachine stateMachine,
            LoadingOperationFactory loadingOperationFactory)
        {
            _stateMachine = stateMachine;
            _loadingOperationFactory = loadingOperationFactory;
        }

        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            Queue<ILoadingOperation> parallelOperations = new Queue<ILoadingOperation>();
            
            parallelOperations.Enqueue(_loadingOperationFactory.CreateInitialAgeIconsLoadingOperation());
            parallelOperations.Enqueue(_loadingOperationFactory.CreateInitialAmbienceLoadingOperation());
            parallelOperations.Enqueue(_loadingOperationFactory.CreateInitialWarriorIconsLoadingOperation());
            parallelOperations.Enqueue(_loadingOperationFactory.CreateShopIconsLoadingOperation());
            
            loadingOperations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Loading resources",parallelOperations));
            _stateMachine.Enter<InitializationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}