using _Game.Core._EngagementTrackers;
using _Game.Core.Services.IAP;
using _Game.Gameplay._Boosts.Scripts;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class TrackersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindBoostsTracker();
            BindBalancyEngagementTracker();
            BindBalancyProgressTracker();
            BindBalancyPaymentTracker();
            BindLocalLevelTracker();
            BindLocalEngagementTracker();
        }

        private void BindBalancyProgressTracker() => 
            Container.BindInterfacesAndSelfTo<BalancyProgressTracker>().AsSingle().NonLazy();

        private void BindBalancyEngagementTracker() => 
            Container.BindInterfacesAndSelfTo<BalacyEngagementTracker>().AsSingle().NonLazy();

        private void BindLocalEngagementTracker() => 
            Container.BindInterfacesAndSelfTo<LocalEngagementTracker>().AsSingle().NonLazy();

        private void BindLocalLevelTracker() => 
            Container.BindInterfacesAndSelfTo<LocalLevelTracker>().AsSingle().NonLazy();

        private void BindBalancyPaymentTracker() => 
            Container.Bind<BalancyPaymentTracker>().AsSingle().NonLazy();

        private void BindBoostsTracker() =>
            Container.BindInterfacesAndSelfTo<BoostsTracker>().AsSingle().NonLazy();
        
    }
}