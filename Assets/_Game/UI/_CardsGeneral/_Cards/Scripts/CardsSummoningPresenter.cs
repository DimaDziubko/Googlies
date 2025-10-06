using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Summoning.Scripts;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsSummoningPresenter
    {
        private SummoningView _view;

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;

        public CardsSummoningPresenter(
            SummoningView view,
            IUserContainer userContainer,
            ICardsConfigRepository cardsConfigRepository,
            IAudioService audioService,
            IWorldCameraService cameraService)
        {
            _view = view;
            _userContainer = userContainer;
            _cardsConfigRepository = cardsConfigRepository;
            _audioService = audioService;
            _cameraService = cameraService;
        }

        public void Initialize()
        {
            CardsState.CardsSummoningProgressChanged += OnSummoningProgressChanged;
            CardsState.CardsSummoningLevelChanged += OnLevelChanged;
            _view.ButtonClicked += OnSummoningClicked;

            OnLevelChanged(CardsState.CardsSummoningLevel);
            OnSummoningProgressChanged(CardsState.CardsSummoningProgressCount);
        }

        private void OnLevelChanged(int level)
        {
            _view.SetLevel(level.ToString());
            OnSummoningProgressChanged(CardsState.CardsSummoningProgressCount);
        }

        private void OnSummoningProgressChanged(int summoning)
        {
            if (IsMaxLevel())
            {
                _view.SetLevel(_cardsConfigRepository.MaxSummoningLevel.ToString());
                _view.SetProgress("max.");
                _view.SetProgress(1);
                return;
            }

            int collectedCardsInCurrentLevel = Mathf.Max(0,
                CardsState.CardsSummoningProgressCount -
                _cardsConfigRepository.GetSummoningForLevel(CardsState.CardsSummoningLevel)
                    .AccumulatedCardsRequiredForLevel);

            int cardsRequiredForNextLevel =
                _cardsConfigRepository.GetSummoningForLevel(CardsState.CardsSummoningLevel + 1).CardsRequiredForLevel;
            
            _view.SetProgress($"{collectedCardsInCurrentLevel}/{cardsRequiredForNextLevel}");
            _view.SetProgress((float)collectedCardsInCurrentLevel / cardsRequiredForNextLevel);

            if (collectedCardsInCurrentLevel >= cardsRequiredForNextLevel)
                _userContainer.UpgradeStateHandler.ChangeCardSummoningLevel(CardsState.CardsSummoningLevel + 1);
        }

        private bool IsMaxLevel() => CardsState.CardsSummoningLevel >= _cardsConfigRepository.MaxSummoningLevel;

        public void Dispose()
        {
            CardsState.CardsSummoningProgressChanged -= OnSummoningProgressChanged;
            CardsState.CardsSummoningLevelChanged -= OnLevelChanged;
            _view.ButtonClicked -= OnSummoningClicked;
        }

        private async void OnSummoningClicked()
        {
            _audioService.PlayButtonSound();

            SummoningPopupPresenter summoningPopupPresenter =
                new SummoningPopupPresenter(CardsState, _cardsConfigRepository);
            ISummoningPopupProvider summoningPopupProvider =
                new SummoningPopupProvider(summoningPopupPresenter, _cameraService, _audioService);
            var popup = await summoningPopupProvider.Load();
            bool isConfirmed = await popup.Value.ShowAndAwaitForExit();
            if (isConfirmed)
            {
                popup.Value.Cleanup();
                summoningPopupProvider.Dispose();
            }
        }
    }
}