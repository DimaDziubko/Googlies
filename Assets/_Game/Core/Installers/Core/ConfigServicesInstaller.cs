using _Game.Core.Configs.Repositories;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class ConfigServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindConfigRepositoryFacade();
        }

        private void BindConfigRepositoryFacade() =>
            Container
                .BindInterfacesAndSelfTo<ConfigRepository>()
                .AsSingle();
        
    }
}