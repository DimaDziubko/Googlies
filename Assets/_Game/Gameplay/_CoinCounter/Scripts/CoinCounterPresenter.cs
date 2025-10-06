using System;
using _Game.Core._GameListenerComposite;
using _Game.Core.UserState._State;
using _Game.UI._Hud;
using _Game.UI._Hud._CoinCounterView;
using _Game.UI.BattleResultPopup.Scripts;

namespace _Game.Gameplay._CoinCounter.Scripts
{
    public class CoinCounterPresenter : IDisposable, IStartGameListener, IEndGameListener
    {
        private readonly ICoinCounter _counter;
        private readonly BattleHud _hud;
        private readonly TemporaryCurrencyBank _temporaryBank;
        public CoinCounterView View => _hud.CoinCounterView;
        private CurrencyCell Cell => _temporaryBank.GetCell(CurrencyType.Coins);
        
        public CoinCounterPresenter(TemporaryCurrencyBank temporaryBank, BattleHud hud)
        {
            _temporaryBank = temporaryBank;
            _hud = hud;
            Init();
        }

        private void Init()
        {
            View.SetActive(false);
            Cell.OnStateChanged += OnCoinsStateChanged;
            View.UpdateCoins(Cell.Amount);
        }

        private void OnCoinsStateChanged() => 
            View.UpdateCoins(Cell.Amount);

        void IDisposable.Dispose() => 
            Cell.OnStateChanged -= OnCoinsStateChanged;

        void IStartGameListener.OnStartBattle()
        {
            View.Clear();
            View.SetActive(true);
        }

        void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit) => View.SetActive(false);
    }
}