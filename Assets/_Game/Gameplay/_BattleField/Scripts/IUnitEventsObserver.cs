using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitEventsObserver
    {
        void NotifyDeath(UnitBase unit);
        void NotifyHit(UnitBase unit, float damage);
        void OnPushOut(UnitBase unit);
    }
}