using _Game.Core._SceneLoader;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Game.Core.GameState
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        
        public BootstrapState(
            IGameStateMachine stateMachine, 
            SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }
        
        public async void Enter()
        {
            var loadOp= _sceneLoader.LoadSceneAsync(Constants.Scenes.STARTUP, LoadSceneMode.Single);
            
            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            
            EnterLoadProgressState();
        }

        private void EnterLoadProgressState()
        {
            _stateMachine.Enter<LoginState>();
        }

        public void Exit()
        {
            
        }
    }
}