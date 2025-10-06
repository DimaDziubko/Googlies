using _Game.Core._Logger;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._MainMenu.State;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class CardsState : ILocalState
    {
        private readonly ICardsScreenProvider _provider;
        private readonly IMyLogger _logger;

        private readonly GeneralCardsScreenPresenter _presenter;

        private Disposable<CardsScreen> _cardsScreen;

        public CardsState(
            ICardsScreenProvider provider,
            GeneralCardsScreenPresenter presenter,
            IMyLogger logger)
        {
            _provider = provider;
            _presenter = presenter;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _cardsScreen = await _provider.Load();
        public void SetActive(bool isActive)
        {
            if (_cardsScreen != null) 
                _cardsScreen.Value.SetActive(isActive);
        }

        public void Enter()
        {
            _presenter.HighlightCardBtn();

            if (_cardsScreen != null)
            {
                _cardsScreen.Value.Show();
            }
        }

        public void Exit()
        {
            if (_cardsScreen?.Value.OrNull() != null)
            {
                _cardsScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _travelScreen is null", DebugStatus.Warning);
            }
            _presenter.UnHighlightCardBtn();
        }

        public void Cleanup() => _provider.Dispose();
    }
}