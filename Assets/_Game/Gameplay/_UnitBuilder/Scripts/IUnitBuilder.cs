using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public interface IUnitBuilder
    {
        void Build(UnitType type, int foodPrice);
    }
}