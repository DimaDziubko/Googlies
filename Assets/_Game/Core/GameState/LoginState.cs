using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Communication;
using _Game.Core.Loading;
using _Game.Core.Login;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.GameState
{
    public class LoginState : IState
    {
        private readonly IUserContainer _userContainer;
        private readonly IUserStateCommunicator _communicator;
        private readonly IGameStateMachine _stateMachine;
        private readonly IMyLogger _logger;

        public LoginState(
            IGameStateMachine stateMachine,
            IUserContainer userContainer,
            IUserStateCommunicator communicator,
            IMyLogger logger)
        {
            _stateMachine = stateMachine;
            _userContainer = userContainer;
            _communicator = communicator;
            _logger = logger;
        }
        
        public void Enter()
        {
            var loadingOperations = new Queue<ILoadingOperation>();
            
            loadingOperations.Enqueue(new LoginOperation(
                _userContainer,
                _communicator,
                _logger));
            
            _stateMachine.Enter<ConfigurationState, Queue<ILoadingOperation>>(loadingOperations);
        }

        public void Exit()
        {
            
        }
    }
}