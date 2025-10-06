using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._Units.Factory
{
    public interface IUnitFactory
    {
        UniTask<UnitBase> GetAsync(UnitType type, Skin skin, Faction faction);
        public void Reclaim(UnitBase unit);
    }
}