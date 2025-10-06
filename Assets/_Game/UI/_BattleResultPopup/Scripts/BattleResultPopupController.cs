using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Navigation.Battle;
using _Game.Core.UserState._State;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._BattleResultPopup.Scripts
{
    public class BattleResultPopupController
    {
        private readonly IBattleResultPopupProvider _battleResultPopupProvider;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly IFeatureUnlockSystem _featureUnlock;
        private readonly IBattleNavigator _battleNavigator;

        private CurrencyCell CoinsCell => _temporaryBank.GetCell(CurrencyType.Coins);
        
        public BattleResultPopupController(
            IBattleResultPopupProvider battleResultPopupProvider,
            TemporaryCurrencyBank temporaryBank,
            IFeatureUnlockSystem featureUnlock,
            IBattleNavigator battleNavigator)
        {
            _battleResultPopupProvider = battleResultPopupProvider;
            _temporaryBank = temporaryBank;
            _featureUnlock = featureUnlock;
            _battleNavigator = battleNavigator;
        }
        
        public async UniTask<bool> ShowGameResultAndWaitForDecision(GameResultType result, bool wasExit = false)
        {
            if(CoinsCell.Amount.Approx(0) && !wasExit && _battleNavigator.CurrentBattleIdx == 0) 
                CoinsCell.Add(Constants.Money.MIN_COINS_PER_BATTLE);
            
            if (CoinsCell.Amount > 0 && _featureUnlock.IsFeatureUnlocked(Feature.X2))
            {
                var popup = await _battleResultPopupProvider.Load();
                var isExitConfirmed = await popup.Value.ShowAndAwaitForExit(result);
                _battleResultPopupProvider.Dispose();
                return isExitConfirmed;
            }
            
            return true; 
        }
    }
}