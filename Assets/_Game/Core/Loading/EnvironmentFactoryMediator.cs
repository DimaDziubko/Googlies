using _Game.UI._Environment.Factory;

namespace _Game.Core.Loading
{
    public class EnvironmentFactoryMediator
    {
        private EnvironmentFactory _environmentFactory;

        public void Initialize(EnvironmentFactory environmentFactory) => 
            _environmentFactory = environmentFactory;

        public void ClearEnvironment() => _environmentFactory?.Cleanup();
    }
}