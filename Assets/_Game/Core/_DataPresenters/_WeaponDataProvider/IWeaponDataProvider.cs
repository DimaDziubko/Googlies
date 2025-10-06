using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core._DataPresenters._WeaponDataProvider
{
    public interface IWeaponDataProvider
    {
        IWeaponData GetWeaponData(int weaponId, Faction faction);
    }
}