using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBattleEventHandler
    {
    }

    public interface IBaseDestructionStartHandler : IBattleEventHandler
    {
        void OnBaseDestructionStarted(Faction faction, Base @base);
    }

    public interface IBaseDestructionCompleteHandler : IBattleEventHandler
    {
        void OnBaseDestructionCompleted(Faction faction, Base @base);
    }
    
    public interface IAllEnemiesDefeatedHandler : IBattleEventHandler
    {
        void OnAllUnitsDefeated();
    }
}