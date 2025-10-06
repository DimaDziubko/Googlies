using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitSpawnProxy
    {
        UniTask SpawnPlayerUnit(UnitType type, Skin skin);
        UniTask SpawnEnemyUnit(UnitType type, Skin skin);
    }
}