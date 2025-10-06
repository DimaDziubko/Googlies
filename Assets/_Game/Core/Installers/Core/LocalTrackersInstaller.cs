using _Game.Core.Services.Analytics;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class LocalTrackersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LocalWaveTraker>()
            .AsSingle()
            .NonLazy();
        }
    }
}
