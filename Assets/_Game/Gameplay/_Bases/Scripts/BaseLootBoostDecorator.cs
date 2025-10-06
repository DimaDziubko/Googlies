namespace _Game.Gameplay._Bases.Scripts
{
    public class BaseLootBoostDecorator : BaseDataDecorator
    {
        private readonly float _multiplier;
        
        public BaseLootBoostDecorator(IBaseData baseData, float multiplier) : base(baseData)
        {
            _multiplier = multiplier;
        }

        public override float CoinsAmount => base.CoinsAmount * _multiplier;
    }
}