using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI.RateGame.Scripts
{
    public interface IRateGameChecker
    {
    }

    public class RateGameChecker : IRateGameChecker, IDisposable
    {
        private const string PP_RATE_GAME_CLICKED = "is_rate_game_clicked_save";

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IRateGameProvider _rateGameProvider;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineStateReadonly => _userContainer.State.TimelineState;

        public RateGameChecker(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IRateGameProvider rateGameProvider,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _rateGameProvider = rateGameProvider;
            _logger = logger;

            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            Debug.Log("RateGameChecker init");
            TimelineStateReadonly.NextBattleOpened += OnNextBattleOpen;
        }

        public void Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
            TimelineStateReadonly.NextBattleOpened -= OnNextBattleOpen;
        }

        private void OnNextBattleOpen()
        {
            if (IsTimeToShow() && !IsReviewed())
            {
                ShowScreen();
            }
        }

        private bool IsReviewed() => PlayerPrefs.HasKey(PP_RATE_GAME_CLICKED);

        private bool IsTimeToShow()
        {
            return TimelineStateReadonly.TimelineId == 0 && TimelineStateReadonly.MaxBattle == 1 ||
                   TimelineStateReadonly.TimelineId == 0 && TimelineStateReadonly.MaxBattle == 2 ||
                   TimelineStateReadonly.TimelineId == 1 && TimelineStateReadonly.MaxBattle == 1;
        }

        private async void ShowScreen()
        {
            var screen = await _rateGameProvider.Load();
            var presenter = new RateGamePresenter(screen.Value);
            presenter.OnSetPP += SetPPValue;

            var isDecision = await screen.Value.AwaitForDecision();
            if (isDecision)
            {
                presenter.OnSetPP -= SetPPValue;
                presenter.Dispose();
                screen.Dispose();
            }
        }

        private void SetPPValue()
        {
            Debug.Log("Rate Game SetPPValue True");
            PlayerPrefs.SetString(PP_RATE_GAME_CLICKED, "true");
        }
    }
}