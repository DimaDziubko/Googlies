using _Game.Core.GameState;
using Zenject;

namespace _Game.Core._StateFactory
{
    public class StateFactory 
    {
        private readonly DiContainer _container;

        public StateFactory(DiContainer container) =>
            _container = container;

        public T CreateState<T>() where T : IExitableState =>
            _container.Resolve<T>();
    }
}