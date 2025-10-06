using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Loading;

namespace _Game.Core.GameState
{
    public class InitializationState : IPayloadedState<Queue<ILoadingOperation>>
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        public InitializationState(
            IGameStateMachine stateMachine,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _logger = logger;
            _stateMachine = stateMachine;
            _gameInitializer = gameInitializer;
        }
        
        public void Enter(Queue<ILoadingOperation> loadingOperations)
        {
            loadingOperations.Enqueue(
                new InitializationOperation(_gameInitializer, _logger));
            
            _stateMachine.Enter<BattleLoadingState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}