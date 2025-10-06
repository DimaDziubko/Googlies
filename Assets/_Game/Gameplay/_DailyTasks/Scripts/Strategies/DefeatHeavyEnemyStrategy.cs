using System;
using System.Linq;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._GameEventRouter;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public class DefeatHeavyEnemyStrategy : IDailyTaskStrategy
    {
        public event Action Completed;

        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;
        private readonly IConfigRepository _config;
        private readonly IBattleNavigator _navigator;
        private readonly BattleField _battleField;
        private readonly IUserContainer _userContainer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineNavigator _timelineNavigator;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;

        private DailyTaskPresenter _presenter;
        private readonly DailyTaskPresenter.Factory _factroy;
        private readonly GameEventRouter _router;
        private bool _isBattleRunning;

        public DefeatHeavyEnemyStrategy(
            DailyTaskModel model,
            DailyTaskView view,
            IConfigRepository config,
            IBattleNavigator navigator,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            BattleField battleField,
            IUserContainer userContainer,
            DailyTaskPresenter.Factory factory,
            GameEventRouter router)
        {
            _router = router;
            _model = model;
            _view = view;
            _config = config;
            _navigator = navigator;
            _ageNavigator = ageNavigator;
            _battleField = battleField;
            _userContainer = userContainer;
            _factroy = factory;
            _timelineNavigator = timelineNavigator;
        }

        public void Initialize()
        {
            _navigator.BattleChanged += OnBattleChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _battleField.EnemyUnitSpawner.UnitDead += OnEnemyDefeated;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _router.BattleStarted += OnBattleStarted;
            _router.BattleStopped += OnBattleStopped;
            OnBattleChanged();
            DefineTarget();
        }

        private void OnTimelineChanged() => OnAgeChanged();

        private void OnAgeChanged()
        {
            if (!_model.IsReady)
            {
                DefineTarget();
                _model.State.ResetProgress();
            }
        }

        public void Execute()
        {
            _presenter = _factroy.Create(_model, _view);
            _presenter.Initialize();
            _presenter.CompleteClicked += OnCompleteClicked;
        }

        private void OnCompleteClicked() => Completed?.Invoke();

        public void Dispose()
        {
            _battleField.EnemyUnitSpawner.UnitDead -= OnEnemyDefeated;
            _presenter.CompleteClicked -= OnCompleteClicked;
            _presenter?.Dispose();
            _navigator.BattleChanged -= OnBattleChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _router.BattleStarted -= OnBattleStarted;
            _router.BattleStopped -= OnBattleStopped;
        }

        private void OnEnemyDefeated(UnitBase unit)
        {
            if (unit.Type == UnitType.Heavy && !_model.IsReady && _isBattleRunning)
            {
                _model.State.AddProgress(1);
            }
        }

        private void OnBattleChanged() => 
            SetAdditionalDescription();

        private void SetAdditionalDescription()
        {
            var additionalDescription = TimelineConfig
                .GetBattleRelatedWarrior(TimelineState.TimelineId, _navigator.CurrentBattleIdx, UnitType.Heavy).Name;
            _model.SetAdditionalDescription(additionalDescription);
        }

        private void DefineTarget()
        {
            int argument = TimelineState.GetUpgradeLevel(UpgradeItemType.FoodProduction);

            var targetFunction = _model.TargetFunctions.FirstOrDefault();

            if (targetFunction != null) _model.SetTarget(targetFunction.GetInt(argument));
            else
            {
                int defaultTargetValue = 1;
                _model.SetTarget(defaultTargetValue);
            }
        }
        
        private void OnBattleStarted() => _isBattleRunning = true;
        private void OnBattleStopped() => _isBattleRunning = false;
    }
}