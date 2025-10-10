using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Buyer;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.UI.Global;
using UnityUtils;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenPresenter :
        ICardsScreenPresenter,
        IDisposable,
        IGameScreenEvents<ICardsScreen>,
        IGameScreenListener<IMenuScreen>,
        ICardsScreen
    {
        public event Action<ICardsScreen> ScreenOpened;
        public event Action<ICardsScreen> InfoChanged;
        public event Action<ICardsScreen> RequiresAttention;
        public event Action<ICardsScreen> ScreenClosed;
        public event Action<ICardsScreen, bool> ActiveChanged;
        public event Action<ICardsScreen> ScreenDisposed;

        public bool IsReviewed { get; private set; }
        public bool NeedAttention => _productBuyer.CanBuy(_x1CardsBundle) ||
                                     _productBuyer.CanBuy(_x10CardsBundle) ||
                                     CardsState.Cards.Any(x => x.Count >= _cardsConfigRepository.ForCard(x.Id).GetUpgradeCount(x.Level));

        //public string Info => $"Cards\n<style=Smaller>{CardsState.Cards.Count}/{_cardsConfigRepository.GetAllCardsCount()}</style>";
        public string Info => $"Cards\n{CardsState.Cards.Count}/{_cardsConfigRepository.GetAllCardsCount()}";

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IIconConfigRepository _iconConfig;
        private readonly IGameInitializer _gameInitializer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IUIFactory _factory;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly ITutorialManager _tutorialManager;
        private readonly IUINotifier _uiNotifier;
        private readonly BoostContainer _boosts;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;

        public CardsScreen Screen { get; set; }

        private Dictionary<Card, CardItemPresenter> _presenters = new();

        private IProductBuyer _productBuyer;

        private CurrencyBank _bank;
        private CurrencyCell Cell { get; set; }

        private CardsSummoningPresenter _cardsSummoningPresenter;

        private CardsBundle _x1CardsBundle;
        private CardsBundle _x10CardsBundle;


        private readonly CardCollector _collector;
        private readonly CardContainer _container;

        public CardsScreenPresenter(
            IUserContainer userContainer,
            IConfigRepository facade,
            IGameInitializer gameInitializer,
            IWorldCameraService cameraService,
            IMyLogger logger,
            IFeatureUnlockSystem featureUnlockSystem,
            IUIFactory factory,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            BoostContainer boosts,
            IUINotifier uiNotifier,
            CurrencyBank bank,
            CardContainer container,
            CardCollector collector,
            IProductBuyer productBuyer)
        {
            _productBuyer = productBuyer;
            _container = container;
            _collector = collector;
            _bank = bank;
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            _cardsConfigRepository = facade.CardsConfigRepository;
            _iconConfig = facade.IconConfigRepository;
            _featureUnlockSystem = featureUnlockSystem;
            _factory = factory;
            _logger = logger;
            _cameraService = cameraService;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _boosts = boosts;
            _uiNotifier = uiNotifier;

            gameInitializer.OnPostInitialization += Init;

            _uiNotifier.RegisterScreen(this, this);
        }

        public void ProductBought(IProduct product)
        {
            if (product is CardsBundle cardsBundle)
            {
                _collector.Collect(cardsBundle.Quantity);
            }
            else
            {
                _logger.Log($"Wrong type {product.Title}", DebugStatus.Warning);
            }
        }

        private void Init()
        {
            Cell = _bank.GetCell(CurrencyType.Gems);

            _x1CardsBundle = new CardsBundle(1, new[]
            {
                new CurrencyData()
                {
                    Type = CurrencyType.Gems,
                    Amount = _cardsConfigRepository.GetX1CardPrice(),
                    Source = ItemSource.Cards
                },
            }
            , ItemSource.CardsScreen);

            _x10CardsBundle = new CardsBundle(10, new[]
            {
                new CurrencyData()
                {
                    Type = CurrencyType.Gems,
                    Amount = _cardsConfigRepository.GetX10CardPrice(),
                    Source = ItemSource.Cards
                },
            }, ItemSource.CardsScreen);


            _productBuyer.ProductBought += ProductBought;
            Cell.OnStateChanged += OnCurrencyStateChanged;
            _container.CardAdded += OnCardCollected;
            Cell.OnAmountAdded += OnStateChanged;

            IsReviewed = !NeedAttention;
            OnStateChanged(0);
        }

        private void OnStateChanged(double _)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        void IDisposable.Dispose()
        {
            _container.CardAdded -= OnCardCollected;
            _gameInitializer.OnPostInitialization -= Init;
            _productBuyer.ProductBought -= ProductBought;
            Cell.OnStateChanged -= OnCurrencyStateChanged;
            Cell.OnAmountAdded -= OnStateChanged;
            _uiNotifier.UnregisterScreen(this);
        }

        private void OnCardCollected(CardModel cardModel)
        {
            if (Screen.OrNull() != null)
            {
                AddCard(cardModel);
            }
        }

        void ICardsScreenPresenter.OnCardScreenOpened()
        {
            if (Screen.OrNull() != null)
            {
                InitButtons();
                InitCards();
                InitSummoning();
                IsReviewed = true;
                ScreenOpened?.Invoke(this);

                _tutorialManager.Register(Screen.CardsStep);

                if (_productBuyer.CanBuy(_x10CardsBundle))
                    Screen.CardsStep.ShowStep();
            }
        }

        void ICardsScreenPresenter.OnCardScreenClosed()
        {
            ScreenClosed?.Invoke(this);
            Cleanup();
        }

        void ICardsScreenPresenter.OnCardScreenActiveChanged(bool isActive) => 
            ActiveChanged?.Invoke(this, isActive);
        

        private void InitCards()
        {
            foreach (var card in _container.GetAll())
            {
                card.SetNew(false);
                AddCard(card);
            }

            SortCards();
        }

        private void AddCard(CardModel model)
        {
            if (_presenters.ContainsKey(model.Card)) return;

            UnityEngine.Transform parent = null;
            if (Screen != null)
            {
                parent = Screen.Container.Transform;
                Screen.Container.AddCard();
            }

            CardItemPresenter presenter = new CardItemPresenter(
                model,
                _factory.GetCard(parent),
                _cameraService,
                _audioService,
                _iconConfig,
                _boosts,
                _logger);
            presenter.Initialize();
            _presenters.Add(model.Card, presenter);
        }

        private void SortCards()
        {
            _presenters = _presenters
                .OrderBy(x => x.Key.Id)
                .ToDictionary(x => x.Key, x => x.Value);

            int i = 0;

            foreach (var presenter in _presenters.Values)
            {
                presenter.View.Transform.SetSiblingIndex(i);
                i++;
            }
        }

        private void InitSummoning()
        {
            _cardsSummoningPresenter = new CardsSummoningPresenter(
                Screen.SummoningView,
                _userContainer,
                _cardsConfigRepository,
                _audioService,
                _cameraService);

            _cardsSummoningPresenter.Initialize();
        }

        private void Cleanup()
        {
            foreach (var presenter in _presenters.Values)
            {
                presenter.Dispose();
                presenter.View.Release();
            }

            _presenters.Clear();

            if (Screen.OrNull() != null)
            {
                Screen.CardsStep.CancelStep();
                _tutorialManager.UnRegister(Screen.CardsStep);
                Screen.X1CardBtn.ButtonClicked -= TryToBuyX1Card;
                Screen.X10CardBtn.ButtonClicked -= TryToBuyX10Card;

                Screen.Container.RemoveCards();
            }

            _cardsSummoningPresenter.Dispose();
        }


        private void InitButtons()
        {
            Screen.X1CardBtn.SetActive(true);
            Screen.X10CardBtn.SetActive(true);

            Screen.X1CardBtn.SetPrice(_cardsConfigRepository.GetX1CardPrice().ToString());
            Screen.X10CardBtn.SetPrice(_cardsConfigRepository.GetX10CardPrice().ToString());
            
            Screen.X1CardBtn.SetCurrencyIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.Gems));
            Screen.X10CardBtn.SetCurrencyIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.Gems));

            CheckButtonsState();

            Screen.X1CardBtn.ButtonClicked += TryToBuyX1Card;
            Screen.X10CardBtn.ButtonClicked += TryToBuyX10Card;
        }

        private void TryToBuyX1Card()
        {
            if (_productBuyer.Buy(_x1CardsBundle))
            {
                _logger.Log("Card X1 successfully bought", DebugStatus.Success);
            }
        }


        private void TryToBuyX10Card()
        {
            if (_productBuyer.Buy(_x10CardsBundle))
            {
                Screen.CardsStep.CompleteStep();

                _logger.Log("Card X10 successfully bought", DebugStatus.Success);
            }
        }

        private void OnCurrencyStateChanged() => CheckButtonsState();


        private void CheckButtonsState()
        {
            if (Screen.OrNull() != null)
            {
                bool isX1CardActive = _featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)
                                      && _productBuyer.CanBuy(_x1CardsBundle);

                Screen.X1CardBtn.SetInteractable(isX1CardActive);

                bool isX10CardActive = _productBuyer.CanBuy(_x10CardsBundle);
                if (isX10CardActive) Screen.CardsStep.ShowStep();
                Screen.X10CardBtn.SetInteractable(isX10CardActive);
            }
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