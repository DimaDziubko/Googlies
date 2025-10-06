using System;
using System.Linq;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Zenject;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UpgradeItemPresenter
    {
        public event Action Upgraded;
        
        private UpgradeItemModel _model;
        private UpgradeItemView _view;
        private readonly IUserContainer _userContainer;
        private readonly IAudioService _audioService;
        private readonly IMiniShopProvider _miniShopProvider;
        private readonly IMyLogger _logger;
        private readonly IIconConfigRepository _config;
        
        private CurrencyCell Cell { get; set; }
        public bool IsReady => _bank.IsEnough(_model.Price);

        private ProductBuyer _productBuyer;
        
        private readonly CurrencyBank _bank;

        public UpgradeItemPresenter(
            UpgradeItemModel model,
            UpgradeItemView view,
            IAudioService audioService,
            IMyLogger logger,
            IUserContainer userContainer,
            IMiniShopProvider miniShopProvider,
            IConfigRepository configRepository,
            CurrencyBank bank)
        {
            _bank = bank;
            _model = model;
            _view = view;
            _userContainer = userContainer;
            _logger = logger;
            _audioService = audioService;
            _miniShopProvider = miniShopProvider;
            _config = configRepository.IconConfigRepository;
        }

        public void SetData(UpgradeItemModel model, UpgradeItemView view)
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            Cell = _bank.GetCell(CurrencyType.Coins);
            _productBuyer = new ProductBuyer(_bank, _logger);
            
            _productBuyer.ProductBought += ProductBought;
            
            OnModelChanged();

            _view.SetIcon(_config.GetIcon(_model.Type));
            _view.SetUpgradeName(_model.Name);
            _view.Button.ShowCurrencyIcon();
            _view.SetCurrencyIcon(_config.GetCurrencyIconFor(CurrencyType.Coins));
            
            Cell.OnStateChanged += OnMoneyChanged;

            _model.Changed += OnModelChanged;
            _view.Button.ButtonClicked += OnButtonClicked;
            _view.Button.InactiveClicked += OnInactiveClicked;
            _view.Button.HoldClicked += OnButtonClicked;

            _view.Button.SetActive(true);
        }

        private void OnButtonClicked()
        {
            _audioService.PlayButtonSound();
            _productBuyer.Buy(_model);

            Upgraded?.Invoke();
        }

        public void Cleanup()
        {
            _view.Button.ButtonClicked -= OnButtonClicked;
            _view.Button.InactiveClicked -= OnInactiveClicked;
            _view.Button.HoldClicked -= OnButtonClicked;
            _productBuyer.ProductBought -= ProductBought;
            
            Cell.OnStateChanged -= OnMoneyChanged;
            
            _model.Changed -= OnModelChanged;
        }

        private void OnMoneyChanged() =>
            _view.Button.SetInteractable(_productBuyer.CanBuy(_model));

        private void ProductBought(IProduct product)
        {
            if (product is UpgradeItemModel model)
            {
                _userContainer.UpgradeStateHandler.UpgradeItem(model.Type);
            }
            else
            {
                _logger.Log($"Wrong type {product.Title}", DebugStatus.Warning);
            }
        }

        public void Dispose()
        {
            Cleanup();
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

        private void OnModelChanged()
        {
            string value = _model.Type == UpgradeItemType.FoodProduction
                ? $"{_model.Value.ToCompactFormat()}/s"
                : _model.Value.ToCompactFormat();
                
            _view.SetValue(value);
            
            _view.Button.SetPrice(_model.Price.FirstOrDefault().Amount.ToCompactFormat());
            
            _view.Button.SetInteractable(_productBuyer.CanBuy(_model));
        }

        public sealed class Factory : PlaceholderFactory<UpgradeItemModel, UpgradeItemView, UpgradeItemPresenter>
        {

        }
    }
}