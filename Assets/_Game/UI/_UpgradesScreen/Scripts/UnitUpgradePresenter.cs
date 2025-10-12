using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._StatsPopup._Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Sirenix.OdinInspector;
using Zenject;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UnitUpgradePresenter
    {
        private UnitUpgrade _model;
        private UnitUpgradeView _view;
        private readonly IUserContainer _userContainer;
        private readonly IIconConfigRepository _config;
        private readonly IMiniShopProvider _miniShopProvider;
        private readonly IAudioService _audioService;
        private readonly IStatsPopupProvider _provider;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private CurrencyCell Cell { get; set; }
        private ProductBuyer _productBuyer;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<StatType, string> _shownStats = new(2);

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<StatType, StatInfoView> _stats = new(2);

        private readonly CurrencyBank _bank;

        public UnitUpgradePresenter(
            UnitUpgrade model,
            UnitUpgradeView view,
            IUserContainer userContainer,
            IConfigRepository config,
            IAudioService audioService,
            IMiniShopProvider miniShopProvider,
            IStatsPopupProvider provider,
            IMyLogger logger,
            CurrencyBank bank)
        {
            _bank = bank;
            _model = model;
            _view = view;
            _userContainer = userContainer;
            _config = config.IconConfigRepository;
            _miniShopProvider = miniShopProvider;
            _audioService = audioService;
            _provider = provider;
            _logger = logger;
        }
        public void SetData(UnitUpgrade model, UnitUpgradeView view)
        {
            _model = model;
            _view = view;

            _logger.Log("SET DATA", DebugStatus.Warning);
        }

        public void Initialize()
        {
            Cell = _bank.GetCell(CurrencyType.Coins);
            _productBuyer = new ProductBuyer(_bank, _logger);

            _productBuyer.ProductBought += ProductBought;

            TimelineState.OpenedUnit += OnUnitOpened;
            Cell.OnStateChanged += OnCurrenciesChanged;
            _view.TransactionButton.ButtonClicked += OnButtonClicked;
            _view.TransactionButton.InactiveClicked += OnInactiveClicked;

            OnUnitOpened(_model.Type);
            _view.SetName(_model.Name);
            _view.SetUnitType(_config.GetUnitType(_model.GetWeaponType()));
            _view.SetIcon(_model.Icon);
            _view.SetCurrencyIcon(_config.GetCurrencyIconFor(CurrencyType.Coins));

            _view.TransactionButton.SetPrice(_model.Price.FirstOrDefault().Amount.ToCompactFormat());
            _view.TransactionButton.SetInteractable(_productBuyer.CanBuy(_model));
            _view.TransactionButton.ShowCurrencyIcon();

            InitStatInfo();

            _view.InfoClicked += OnInfoClicked;
        }

        public void Dispose()
        {
            Cleanup();
        }
        public void Cleanup()
        {
            TimelineState.OpenedUnit -= OnUnitOpened;
            Cell.OnStateChanged -= OnCurrenciesChanged;
            _view.TransactionButton.ButtonClicked -= OnButtonClicked;
            _view.TransactionButton.InactiveClicked -= OnInactiveClicked;
            _view.InfoClicked -= OnInfoClicked;

            _productBuyer.ProductBought -= ProductBought;

            foreach (var stat in _stats.Values)
                _view.StatInfoListViews.DestroyElement(stat);

            _stats.Clear();
        }

        private void ProductBought(IProduct product)
        {
            if (product is UnitUpgrade model)
            {
                _userContainer.UpgradeStateHandler.OpenUnit(model.Type);
            }
            else
            {
                _logger.Log($"Wrong type {product.Title}", DebugStatus.Warning);
            }
        }

        private async void OnInactiveClicked()
        {
            if (!_miniShopProvider.IsUnlocked) return;
            var popup = await _miniShopProvider.Load();
            var isExit = await popup.Value.ShowAndAwaitForDecision(_model.Price.FirstOrDefault().Amount);
            if (isExit)
            {
                _miniShopProvider.Dispose();
            }
        }

        private void OnButtonClicked()
        {
            _productBuyer.Buy(_model);
            _audioService.PlayUpgradeSound();
        }

        private void InitStatInfo()
        {
            foreach (StatInfo stat in _model.Stats)
            {
                if (!_stats.TryGetValue(stat.Type, out StatInfoView view))
                {
                    view = _view.StatInfoListViews.SpawnElement();
                    _stats.Add(stat.Type, view);
                }

                view.SetIcon(_config.ForStatIcon(stat.Type));

                string statValue = stat.Value.ToCompactFormat();


                if (!_shownStats.TryGetValue(stat.Type, out string shownValue))
                {
                    view.SetValue(statValue);
                    _shownStats.Add(stat.Type, statValue);
                }
                else
                {
                    if (shownValue == statValue)
                    {
                        view.SetValue(statValue);
                    }
                    else
                    {
                        view.AddStat(shownValue, statValue);
                    }
                }

                _shownStats[stat.Type] = statValue;
            }
        }

        private async void OnInfoClicked()
        {
            _audioService.PlayButtonSound();

            var popup = await _provider.Load();
            bool isConfirmed = await popup.Value.ShowStatsAndAwaitForExit(_model.Type);
            if (isConfirmed)
            {
                _provider.Dispose();
            }
        }

        private void OnCurrenciesChanged() =>
            _view.TransactionButton.SetInteractable(_productBuyer.CanBuy(_model));

        private void OnUnitOpened(UnitType type)
        {
            bool isOpened = TimelineState.OpenUnits.Contains(_model.Type);
            _view.SetLocked(!isOpened);
            _view.TransactionButton.SetActive(!isOpened);
        }

        public sealed class Factory : PlaceholderFactory<UnitUpgrade, UnitUpgradeView, UnitUpgradePresenter>
        {

        }
    }
}