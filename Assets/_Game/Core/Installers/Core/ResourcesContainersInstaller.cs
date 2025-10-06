using _Game.Core._IconContainer;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class ResourcesContainersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindAgeIconContainer();
            BindAmbienceContainer();
            BindWarriorIconContainer();
            BindShopIconContainer();
        }

        private void BindShopIconContainer() => 
            Container.Bind<ShopIconsContainer>().AsSingle();

        private void BindWarriorIconContainer() => 
            Container.Bind<WarriorIconContainer>().AsSingle();

        private void BindAmbienceContainer() => 
            Container.Bind<AmbienceContainer>().AsSingle();

        private void BindAgeIconContainer() => 
            Container.Bind<AgeIconContainer>().AsSingle();
    }
}