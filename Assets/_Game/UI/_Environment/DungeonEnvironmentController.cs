using _Game.UI._Environment.Factory;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Environment
{
    public class DungeonEnvironmentController
    {
        private readonly IEnvironmentFactory _factory;

        public DungeonEnvironmentController(IEnvironmentFactory factory)
        {
            _factory = factory;
        }

        public async UniTask Initialize(string environmentKey) => 
            await ShowEnvironment(environmentKey);
        
        private async UniTask ShowEnvironment(string environmentKey) => 
            await _factory.Get(environmentKey);
    }
}