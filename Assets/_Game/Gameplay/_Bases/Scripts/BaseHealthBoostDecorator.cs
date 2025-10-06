namespace _Game.Gameplay._Bases.Scripts
{
    public class BaseHealthBoostDecorator : BaseDataDecorator
    {
        private readonly float _healthMultiplier;
        
        public BaseHealthBoostDecorator(IBaseData baseData, float healthMultiplier) : base(baseData)
        {
            _healthMultiplier = healthMultiplier;
        }

        public override float Health => _baseData.Health * _healthMultiplier;
    }
}