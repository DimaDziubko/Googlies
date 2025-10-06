using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;

namespace _Game.Core.Installers.Local.Common
{
    public class BattleModeFactoriesInstaller : BaseFactoriesInstaller
    {
        protected override IUnitDataProvider GetUnitDataProvider() =>
            Container.Resolve<IBattleModeUnitDataProvider>();

        protected override IWeaponDataProvider GetWeaponDataProvider() =>
            Container.Resolve<IBattleModeWeaponDataProvider>();

        protected override IBaseDataProvider GetBaseDataProvider() =>
            Container.Resolve<IBattleModeBaseDataProvider>();
    }
}
