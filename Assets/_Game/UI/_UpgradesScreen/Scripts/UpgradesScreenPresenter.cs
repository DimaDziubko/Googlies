using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._StatsPopup._Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using UnityUtils;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public struct StatInfo
    {
        public StatType Type;
        public float Value;
        public float BoostValue;
    }

    public class UpgradesScreenPresenter :
        IUpgradesScreenPresenter,
        IDisposable,
        IGameScreenEvents<IUpgradesScreen>,
        IUpgradesScreen,
        IGameScreenListener<IMenuScreen>
    {
        public event Action<IUpgradesScreen> ScreenOpened;
        public event Action<IUpgradesScreen> InfoChanged;
        public event Action<IUpgradesScreen> RequiresAttention;
        public event Action<IUpgradesScreen> ScreenClosed;
        public event Action<IUpgradesScreen, bool> ActiveChanged;
        public event Action<IUpgradesScreen> ScreenDisposed;

        [ShowInInspector, ReadOnly]
        public bool IsReviewed { get; private set; }

        //[ShowInInspector, ReadOnly]
        public bool NeedAttention => UpgradeItems.Any(x => _bank.IsEnough(x.Price))
                                     || Units.Any(x => _bank.IsEnough(x.Price) && !TimelineState.OpenUnits.Contains(x.Type));
        public string Info => "Upgrades";
        public UpgradesScreen Screen { get; set; }

        private readonly UpgradeItemPresenter.Factory _upgradeItemPresenterFactory;
        private readonly UnitUpgradePresenter.Factory _unitUpgradePresenterFactory;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<UnitType, UnitUpgrade> _unitUpgradeModelsCache = new(3);

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<UpgradeItemType, UpgradeItemPresenter> _upgradeItemPresenters = new(2);

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<UnitType, UnitUpgradePresenter> _unitUpgradePresenters = new(3);
        private readonly QuickBoostInfoPresenter.Factory _factory;

        private IEnumerable<UpgradeItemModel> UpgradeItems => _upgradeItemContainer.GetAllItems();

        private IEnumerable<IUnitData> Units => _unitDataProvider.GetAllPlayerUnits();

        private readonly UpgradeItemContainer _upgradeItemContainer;
        private readonly IBattleModeUnitDataProvider _unitDataProvider;
        private readonly IUINotifier _uiNotifier;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly ITutorialManager _tutorialManager;

        private readonly CurrencyBank _bank;

        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private CurrencyCell Cell => _bank.GetCell(CurrencyType.Coins);
        private QuickBoostInfoPresenter _quickBoostPresenter;

        public UpgradesScreenPresenter(
            UpgradeItemPresenter.Factory upgradeItemPresenterFactory,
            UnitUpgradePresenter.Factory unitUpgradePresenterFactory,
            UpgradeItemContainer upgradeItemContainer,
            IBattleModeUnitDataProvider unitDataProvider,
            IUINotifier uiNotifier,
            QuickBoostInfoPresenter.Factory factory,
            IGameInitializer gameInitializer,
            CurrencyBank bank,
            ITutorialManager tutorialManager,
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _userContainer = userContainer;
            _bank = bank;
            _upgradeItemContainer = upgradeItemContainer;
            _unitDataProvider = unitDataProvider;
            _upgradeItemPresenterFactory = upgradeItemPresenterFactory;
            _unitUpgradePresenterFactory = unitUpgradePresenterFactory;
            _uiNotifier = uiNotifier;
            _gameInitializer = gameInitializer;
            _factory = factory;
            _logger = logger;
            _tutorialManager = tutorialManager;

            gameInitializer.OnPostInitialization += Init;

            _uiNotifier.RegisterScreen(this, this);
        }

        private void Init()
        {
            Cell.OnAmountAdded += OnStateChanged;

            IsReviewed = !NeedAttention;

            OnStateChanged(0);
        }

        void IDisposable.Dispose()
        {
            Cell.OnAmountAdded -= OnStateChanged;
            _gameInitializer.OnPostInitialization -= Init;
            _uiNotifier.UnregisterScreen(this);

            DisposeUpgradeItems();
            DisposeUnitUpgrades();

            _unitUpgradePresenters.Clear();
            _upgradeItemPresenters.Clear();
        }

        private void OnStateChanged(double _)
        {
            _logger.Log("UPGRADES PRESENTER ON STATE CHANGED", DebugStatus.Warning);
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
                _logger.Log("UPGRADES PRESENTER REQUIRES ATTENTION", DebugStatus.Warning);
            }
        }

        void IUpgradesScreenPresenter.OnUpgradesScreenOpened()
        {
            if (Screen.OrNull() != null)
            {
                UpdateItems();
                IsReviewed = true;
                ScreenOpened?.Invoke(this);
                InitQuickBoostInfo();

                if (_tutorialManager.Register(Screen.FoodStep))
                {
                    var foodUpgradeItemPresenter = _upgradeItemPresenters[UpgradeItemType.FoodProduction];

                    if (foodUpgradeItemPresenter.IsReady)
                    {
                        Screen.FoodStep.ShowStep();
                    }

                    foodUpgradeItemPresenter.Upgraded += OnFoodUpgraded;
                }
            }
        }

        private void InitQuickBoostInfo()
        {
            if (_quickBoostPresenter != null)
            {
                _quickBoostPresenter.Dispose();
                _quickBoostPresenter.SetView(Screen.QuickBoostInfoPanel);
            }
            else
            {
                _quickBoostPresenter = _factory.Create(Screen.QuickBoostInfoPanel);
            }

            _quickBoostPresenter.Initialize();
        }

        private void OnFoodUpgraded() => Screen.FoodStep.CompleteStep();

        private void DisposeUpgradeItems()
        {
            foreach (var presenter in _upgradeItemPresenters.Values)
            {
                presenter.Dispose();
            }
        }

        private void DisposeUnitUpgrades()
        {
            foreach (var presenter in _unitUpgradePresenters.Values)
            {
                presenter.Dispose();
            }
        }

        void IUpgradesScreenPresenter.OnUpgradesScreenClosed()
        {
            if (_upgradeItemPresenters.TryGetValue(UpgradeItemType.FoodProduction, out var upgradeItemPresenter))
                upgradeItemPresenter.Upgraded -= OnFoodUpgraded;

            Screen.FoodStep.CancelStep();

            _tutorialManager.UnRegister(Screen.FoodStep);

            ScreenClosed?.Invoke(this);
            Cleanup();
        }

        void IUpgradesScreenPresenter.OnUpgradesScreenDisposed()
        {
            if (_upgradeItemPresenters.TryGetValue(UpgradeItemType.FoodProduction, out var upgradeItemPresenter))
                upgradeItemPresenter.Upgraded -= OnFoodUpgraded;

            Screen.FoodStep.CancelStep();

            _tutorialManager.UnRegister(Screen.FoodStep);

            Cleanup();

            ScreenDisposed?.Invoke(this);
        }

        void IUpgradesScreenPresenter.OnScreenActiveChanged(bool isActive) =>
            ActiveChanged?.Invoke(this, isActive);

        private void Cleanup()
        {
            ClearUnitUpgrades();
            ClearUpgradeItems();
        }

        private void ClearUnitUpgrades()
        {
            foreach (var presenter in _unitUpgradePresenters.Values)
            {
                presenter.Cleanup();
            }
        }

        private void ClearUpgradeItems()
        {
            foreach (var presenter in _upgradeItemPresenters.Values)
            {
                presenter.Cleanup();
            }
        }

        private void UpdateItems()
        {
            foreach (var unit in Units)
            {
                UpdateUnitUpgrade(unit);
            }

            foreach (var item in UpgradeItems)
            {
                UpdateUpgrade(item);
            }
        }

        private void UpdateUnitUpgrade(IUnitData unit)
        {
            var data = _unitDataProvider.GetDecoratedUnitData(Faction.Player, unit.Type, Skin.Ally);

            if (_unitUpgradeModelsCache.TryGetValue(unit.Type, out var model))
            {
                model.SetData(data);
            }
            else
            {
                model = new UnitUpgrade(data);

                _unitUpgradeModelsCache.Add(unit.Type, model);
            }

            UnitUpgradeView view = Screen.GetUnitUpgrade(unit.Type);

            if (_unitUpgradePresenters.TryGetValue(unit.Type, out var presenter))
            {
                presenter.SetData(model, view);
            }
            else
            {
                presenter = _unitUpgradePresenterFactory.Create(model, view);
                _unitUpgradePresenters.Add(unit.Type, presenter);
            }

            presenter.Initialize();
        }

        private void UpdateUpgrade(UpgradeItemModel model)
        {
            UpgradeItemView view = Screen.GetUpgradeItemView(model.Type);

            if (_upgradeItemPresenters.TryGetValue(model.Type, out var presenter))
            {
                presenter.SetData(model, view);
            }
            else
            {
                presenter = _upgradeItemPresenterFactory.Create(model, view);
                _upgradeItemPresenters.Add(model.Type, presenter);
            }

            presenter.Initialize();
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}