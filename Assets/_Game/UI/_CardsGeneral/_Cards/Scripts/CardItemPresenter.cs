using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Utils.Disposable;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardItemPresenter
    {
       public CardItemView View => _view;
        public CardModel Model => _cardModel;

        private CardModel _cardModel;
        private CardItemView _view;
        
        private readonly CardPresenter _presenter;
        
        private ICardPopupProvider _cardPopupProvider;
        private IWorldCameraService _cameraService;
        private IAudioService _audioService;
        private IIconConfigRepository _config;
        private BoostContainer _boosts;
        private CardPopupPresenter _cardPopupPresenter;
        private IMyLogger _logger;

        public CardItemPresenter(
            CardModel cardModel, 
            CardItemView view,
            IMyLogger logger)
        {
            _cardModel = cardModel;
            _view = view;
            _presenter = new CardPresenter(cardModel, view.CardView);
            _logger = logger;
        }

        public CardItemPresenter(
            CardModel cardModel, 
            CardItemView view,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IMyLogger logger)
        {
            _cardModel = cardModel;
            _view = view;
            _cameraService = cameraService;
            _audioService = audioService;
            _config = config;
            _boosts = boosts;
            _logger = logger;            
            
            _presenter = new CardPresenter(cardModel, view.CardView);
        }

        public void InitializeNotClickable()
        {
            _presenter.Initialize();
            _cardModel.Card.CountChanged += OnStateChanged;
            _cardModel.Card.OnLevelUp += OnLevelChanged;
            
            OnStateChanged();
        }
        
        public void Initialize()
        {
            _presenter.Initialize();
            _cardModel.Card.CountChanged += OnStateChanged;
            _cardModel.Card.OnLevelUp += OnLevelChanged;
            _view.CardClicked += OnCardClicked;
            
            OnStateChanged();
        }

        private void OnLevelChanged(Card card) => OnStateChanged();

        private void OnStateChanged()
        {
            _view.SetProgressValue(_cardModel.ProgressValue);
            _view.SetProgress($"{_cardModel.Card.Count}/{_cardModel.GetUpgradeCount()}");
            _view.SetActiveUpgradeNotifier(_cardModel.IsReadyForUpgrade);
            _view.SetBarColor(_cardModel.GetBarColorName());
        }

        public void Dispose()
        {
            _presenter.Dispose();
            _cardModel.Card.CountChanged -= OnStateChanged;
            _cardModel.Card.OnLevelUp -= OnLevelChanged;
            _view.CardClicked -= OnCardClicked;
        }

        private async void OnCardClicked()
        {
            _audioService.PlayButtonSound();
            
            _cardModel.SetNew(false);
            _cardPopupPresenter ??= new CardPopupPresenter(Model);
            _cardPopupProvider ??= new CardPopupProvider(
                _cardPopupPresenter,
                _cameraService,
                _audioService,
                _config,
                _boosts,
                _logger);
            Disposable<CardPopup> popup = await _cardPopupProvider.Load();
            bool isConfirmed = await popup.Value.ShowDetailsAndAwaitForExit();
            if (isConfirmed) _cardPopupProvider.Dispose();
        }
    }
}