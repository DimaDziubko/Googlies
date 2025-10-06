using _Game.LiveopsCore._GameEventCurrencyManagement;
using Zenject;

namespace _Game.Core.Installers.Local.Common
{
    public class LocalCoreSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGameEventCurrencyEarningSystem();
        }
        
        private void BindGameEventCurrencyEarningSystem() => 
            Container.BindInterfacesAndSelfTo<GameEventCurrencyEarningSystem>().AsSingle().NonLazy();
    }
}