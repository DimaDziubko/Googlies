using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }

    public interface IVisitor
    {
        void Visit(UnitBase unit);
    }
}