using System;
using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Factory;
using _Game.Core.GameState;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._BattleResultPopup.Scripts;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game._BattleModes.Scripts
{
    public class BattleMode :
        IGameModeCleaner,
        IBaseDestructionStartHandler,
        IBaseDestructionCompleteHandler,
        IDisposable,
        IEndGameListener,
        IStartGameListener,
        IResetable
    {
        public IEnumerable<GameObjectFactory> Factories => _factoryHolder.Factories;
        public string SceneName => Constants.Scenes.BATTLE_MODE;

        private readonly IBattleTriggersManager _battleTriggersManager;

        private readonly IGameManager _gameManager;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ITimelineConfigRepository _timelineConfig;
        private readonly IGameStateMachine _state;
        private readonly IFactoriesHolder _factoryHolder;
        private readonly IMyLogger _logger;

        private readonly Battle _battle;
        private readonly BattleResultPopupController _resultPopup;
        private readonly LoadingOperationFactory _loadingOperationFactory;

        private Action _timelineChangedHandler;
        private Action _ageChangedHandler;
        private Action _battleChangedHandler;

        public BattleMode(
            IGameStateMachine state,
            IBattleTriggersManager battleTriggersManager,
            IFactoriesHolder factoriesHolder,
            IGameManager gameManager,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            IBattleNavigator battleNavigator,
            IConfigRepository configRepository,
            LoadingOperationFactory loadingOperationFactory,
            Battle battle,
            BattleResultPopupController resultPopup,
            IMyLogger logger)
        {
            _state = state;
            _battleTriggersManager = battleTriggersManager;
            _gameManager = gameManager;
            _battle = battle;
            _logger = logger;

            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _battleNavigator = battleNavigator;
            _timelineConfig = configRepository.TimelineConfigRepository;
            _resultPopup = resultPopup;
            _loadingOperationFactory = loadingOperationFactory;
            _factoryHolder = factoriesHolder;
        }

        public async UniTask Initialize()
        {
            _battleTriggersManager.Register(this);

            _timelineChangedHandler = async () => await OnTimelineChanged();
            _ageChangedHandler = async () => await OnAgeChanged();
            _battleChangedHandler = async () => await OnBattleChanged();

            _timelineNavigator.TimelineChanged += _timelineChangedHandler;
            _ageNavigator.AgeChanged += _ageChangedHandler;
            _battleNavigator.BattleChanged += _battleChangedHandler;

            _battle.Initialize();

            await OnTimelineChanged();
        }

        void IDisposable.Dispose()
        {
            if (_timelineChangedHandler != null)
                _timelineNavigator.TimelineChanged -= _timelineChangedHandler;

            if (_ageChangedHandler != null)
                _ageNavigator.AgeChanged -= _ageChangedHandler;

            if (_battleChangedHandler != null)
                _battleNavigator.BattleChanged -= _battleChangedHandler;

            _battle.Dispose();
        }

        async UniTask IResetable.Reset() =>
           await _battle.Reset();

        void IGameModeCleaner.Cleanup() => _battle.Cleanup();

        void IBaseDestructionStartHandler.OnBaseDestructionStarted(Faction faction, Base @base) => 
            _gameManager.StopBattle();

        void IBaseDestructionCompleteHandler.OnBaseDestructionCompleted(Faction faction, Base @base)
        {
            switch (faction)
            {
                case Faction.Player:
                    _gameManager.EndBattle(GameResultType.Defeat);
                    break;
                case Faction.Enemy:
                    _gameManager.EndBattle(GameResultType.Victory);
                    break;
            }
        }

        void IStartGameListener.OnStartBattle()
        {
            _battleNavigator.InitLevel();
            _state.Enter<BattleLoopState>();
        }

        async void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit)
        {
            await _resultPopup.ShowGameResultAndWaitForDecision(result, wasExit);

            Queue<ILoadingOperation> loadingOperations = new Queue<ILoadingOperation>();

            loadingOperations.Enqueue(_loadingOperationFactory.CreateClearModeOperation(this));
            if (result == GameResultType.Victory)
                loadingOperations.Enqueue(_loadingOperationFactory.CreateBattleTransitionOperation(_battleNavigator));
            loadingOperations.Enqueue(_loadingOperationFactory.CreateResetOperation(this));

            GoToMainMenu(loadingOperations);
        }

        private async UniTask OnTimelineChanged()
        {
            _logger.Log($"BATTLE MODE ON TIMELINE CHANGED", DebugStatus.Warning);
            _battle.OnTimelineChanged();
            await OnAgeChanged();
            await OnBattleChanged();
        }

        private async UniTask OnAgeChanged()
        {
            _logger.Log($"BATTLE MODE ON AGE CHANGED", DebugStatus.Warning);
            await _battle.OnAgeChanged();
        }

        private async UniTask OnBattleChanged()
        {
            _logger.Log($"BATTLE MODE ON BATTLE CHANGED", DebugStatus.Warning);
            var battleIdx = Mathf.Min(_battleNavigator.CurrentBattleIdx, 5);
            var battleConfig =
                _timelineConfig.GetRelatedBattle(_timelineNavigator.CurrentTimelineId, battleIdx);
            await _battle.OnBattleChanged(battleConfig);
        }

        private void GoToMainMenu(Queue<ILoadingOperation> operations)
        {
            SimpleLoadingData data = new SimpleLoadingData(LoadingScreenType.DarkFade, operations);
            _state.Enter<MenuState, SimpleLoadingData>(data);
        }
    }
}