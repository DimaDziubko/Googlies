using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;

namespace _Game.Core.Installers.Local.Common
{
    public class ZombieRushFactoriesInstaller : BaseFactoriesInstaller
    {
        protected override IUnitDataProvider GetUnitDataProvider() =>
            Container.Resolve<IZombieRushModeUnitDataProvider>();

        protected override IWeaponDataProvider GetWeaponDataProvider() =>
            Container.Resolve<IZombieRushModeWeaponDataProvider>();

        protected override IBaseDataProvider GetBaseDataProvider() =>
            Container.Resolve<IZombieRushModeBaseDataProvider>();
    }
}