using _Game.Core.UserState._State;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public class CoinCounter : ICoinCounter
    {
        private readonly TemporaryCurrencyBank _temporaryBank;
        private CurrencyCell CoinsCell => _temporaryBank.GetCell(CurrencyType.Coins);
        
        public CoinCounter(TemporaryCurrencyBank temporaryBank)
        {
            _temporaryBank = temporaryBank;
        }
        
        public void AddCoins(float amount) => CoinsCell.Add(amount);
    }
}