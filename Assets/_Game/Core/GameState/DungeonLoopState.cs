using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using Cysharp.Threading.Tasks;

namespace _Game.Core.GameState
{
    public class DungeonLoopState : IPayloadedState<SimpleLoadingData>
    {
        private readonly ILoadingScreenProvider _loadingProvider;
        
        public DungeonLoopState(ILoadingScreenProvider loadingProvider)
        {
            _loadingProvider = loadingProvider;
        }
        
        public void Enter(SimpleLoadingData data)
        {
            _loadingProvider.LoadAndDestroy(data.Operations, data.Type).Forget();
        }
        
        public void Exit()
        {

        }
    }
}