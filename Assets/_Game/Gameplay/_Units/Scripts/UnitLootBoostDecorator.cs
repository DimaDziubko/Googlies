namespace _Game.Gameplay._Units.Scripts
{
    public class UnitLootBoostDecorator : UnitDataDecorator
    {
        private readonly float _multiplier;

        public UnitLootBoostDecorator(IUnitData unitData, float multiplier)
            : base(unitData)
        {
            _multiplier = multiplier;
        }

        public override float CoinsPerKill => base.CoinsPerKill * _multiplier;
    }
}