using _Game.Gameplay._Bases.Factory;

namespace _Game.Core.Loading
{
    public class BaseFactoryMediator
    {
        private BaseFactory _baseFactory;

        public void Initialize(BaseFactory baseFactory) => 
            _baseFactory = baseFactory;
 
        public void ClearBases() => _baseFactory?.Cleanup();
    }
}