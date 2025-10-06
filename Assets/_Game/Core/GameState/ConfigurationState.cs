using System.Collections.Generic;
using _Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class ConfigurationState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly LoadingOperationFactory _factory;

        public ConfigurationState(
            LoadingOperationFactory factory,
            IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _factory = factory;
        }

        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            
            loadingOperations.Enqueue(_factory.CreateConfigOperation());
            loadingOperations.Enqueue(_factory.CreateDungeonsValidationOperation());
            
            _stateMachine.Enter<LoadingResourcesState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}