namespace _Game.Gameplay._Units.Scripts
{
    public class HealthBoostDecorator : UnitDataDecorator
    {
        private readonly float _healthMultiplier;

        public HealthBoostDecorator(IUnitData unitData, float healthMultiplier)
            : base(unitData)
        {
            _healthMultiplier = healthMultiplier;
        }
        
        public override float Health => HealthBoost * base.Health;
        public override float HealthBoost => _healthMultiplier * base.HealthBoost;
    }
    
    public class HealthDecorator : UnitDataDecorator
    {
        private readonly float _healthMultiplier;

        public HealthDecorator(IUnitData unitData, float healthMultiplier)
            : base(unitData)
        {
            _healthMultiplier = healthMultiplier;
        }
        
        public override float Health => _healthMultiplier * base.Health;
    }
}