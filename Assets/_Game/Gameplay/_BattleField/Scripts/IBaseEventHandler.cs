using _Game.Gameplay._Bases.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBaseEventHandler
    {
        void OnBaseHit(Base @base, float damage, float maxHealth);
        void OnBaseDestructionStarted(Base @base);
        void OnBaseDestructionCompleted(Base @base);
    }
}