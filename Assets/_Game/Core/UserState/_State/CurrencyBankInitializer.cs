using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._State
{
    public class CurrencyBankInitializer : IDisposable
    {
        private readonly CurrencyBank _bank;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        public CurrencyBankInitializer(
            IUserContainer userContainer, 
            CurrencyBank bank, 
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _logger = logger;
            gameInitializer.OnPreInitialization += Init;
            _gameInitializer = gameInitializer;
            _bank = bank;
            _userContainer = userContainer;
        }

        private void Init()
        {
            _logger.Log("Initializing currency bank...", DebugStatus.Warning);
            _bank.Bind(_userContainer.State.Currencies.Cells);
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPreInitialization -= Init;
        }
    }
}