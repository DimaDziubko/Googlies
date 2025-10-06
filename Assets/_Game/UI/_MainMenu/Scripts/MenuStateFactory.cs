using System;
using _Game.Core._Logger;
using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._Dungeons.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.UI._UpgradesAndEvolution.Scripts;

namespace _Game.UI._MainMenu.Scripts
{
    public class MenuStateFactory
    {
        private readonly IStartBattleScreenProvider _startBattleScreenProvider;
        private readonly IUpgradeAndEvolutionScreenProvider _upgradeAndEvolutionScreenProvider;
        private readonly IShopProvider _shopProvider;
        private readonly IGeneralCardsScreenProvider _generalCardsScreenProvider;
        private readonly IMyLogger _logger;
        private readonly IDungeonsScreenProvider _dungeonsProvider;

        public MenuStateFactory(
            IStartBattleScreenProvider startBattleScreenProvider,
            IUpgradeAndEvolutionScreenProvider upgradeAndEvolutionScreenProvider,
            IShopProvider shopProvider,
            IGeneralCardsScreenProvider generalCardsScreenProvider,
            IDungeonsScreenProvider dungeonsDungeonsProvider,
            IMyLogger logger)
        {
            _startBattleScreenProvider = startBattleScreenProvider;
            _upgradeAndEvolutionScreenProvider = upgradeAndEvolutionScreenProvider;
            _shopProvider = shopProvider;
            _generalCardsScreenProvider = generalCardsScreenProvider;
            _dungeonsProvider = dungeonsDungeonsProvider;
            _logger = logger;
        }

        public TState CreateState<TState>(MainMenu mainMenu, MenuButton button) where TState : class, ILocalState
        {
            switch (button.Type)
            {
                case MenuButtonType.Battle:
                    return new BattleState(mainMenu, _startBattleScreenProvider, _logger) as TState;
                case MenuButtonType.Upgrades:
                    return new MenuUpgradesState(mainMenu, _upgradeAndEvolutionScreenProvider, _logger) as TState;
                case MenuButtonType.Shop:
                    return new ShopState(mainMenu, _shopProvider, _logger) as TState;
                case MenuButtonType.Cards:
                    return new GeneralCardsState(mainMenu, _generalCardsScreenProvider, _logger) as TState;
                case MenuButtonType.Dungeons:
                    return new DungeonsState(mainMenu, _dungeonsProvider, _logger) as TState;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button.Type), $"No state found for button type {button.Type}");
            }
        }
    }
}