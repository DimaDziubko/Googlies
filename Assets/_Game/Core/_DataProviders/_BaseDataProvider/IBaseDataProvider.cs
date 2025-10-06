using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders._BaseDataProvider
{
    public interface IBaseDataProvider
    {
        IBaseData GetBaseData(Faction faction);
    }
}