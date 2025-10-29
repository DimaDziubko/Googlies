using _Game.Core._EngagementTrackers;
using _Game.Core.Services.Analytics;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class LocalTrackersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindAFLevelProgressTrackerService();
        }

        private void BindAFLevelProgressTrackerService() =>
            Container
           .BindInterfacesAndSelfTo<AFLevelProgressTracker>()
           .AsSingle();
    }
}
