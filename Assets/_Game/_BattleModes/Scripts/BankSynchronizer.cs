using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.LoadingScreen;
using _Game.Core.UserState._State;
using _Game.UI.BattleResultPopup.Scripts;

namespace _Game._BattleModes.Scripts
{
    public class BankSynchronizer : IEndGameListener
    {
        private readonly CurrencyBank _bank;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly ILoadingScreenProvider _provider;
        private readonly IMyLogger _logger;

        public BankSynchronizer(
            TemporaryCurrencyBank temporaryBank,
            CurrencyBank bank,
            ILoadingScreenProvider provider,
            IMyLogger logger)
        {
            _logger = logger;
            _temporaryBank = temporaryBank;
            _bank = bank;
            _provider = provider;
        }
        
        public void OnEndBattle(GameResultType result, bool wasExit)
        {
            _logger.Log("BANK SYNCHRONIZER ON END BATTLE");
            _provider.LoadingCompleted += OnLoadingCompleted;
        }

        private void OnLoadingCompleted()
        {
            _logger.Log("BANK SYNCHRONIZER ON LOADING COMPLETE");
            
            foreach (CurrencyCell cell in _temporaryBank)
            {
                if (cell.Amount > 0)
                {
                    _bank.Add(new CurrencyData()
                    {
                        Type = cell.Type,
                        Amount = (float)cell.Amount
                    });
                    
                    _logger.Log($"BANK SYNCHRONIZER ADD TYPE {cell.Type}, AMOUNT {cell.Amount} ");
                    
                    cell.Change(0);
                }
            }
            
            _provider.LoadingCompleted -= OnLoadingCompleted;
        }
    }
}