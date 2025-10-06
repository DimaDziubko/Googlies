using _Game.Core.GameState;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BootstrapState>().AsSingle().NonLazy();
            Container.Bind<ConfigurationState>().AsSingle().NonLazy();
            Container.Bind<LoginState>().AsSingle().NonLazy();
            Container.Bind<InitializationState>().AsSingle().NonLazy();
            Container.Bind<BattleLoadingState>().AsSingle().NonLazy();
            Container.Bind<MenuState>().AsSingle().NonLazy();
            Container.Bind<BattleLoopState>().AsSingle().NonLazy();
            Container.Bind<DungeonLoopState>().AsSingle().NonLazy();
            Container.Bind<LoadingResourcesState>().AsSingle().NonLazy();

            Container
                .BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();
        }
    }
}