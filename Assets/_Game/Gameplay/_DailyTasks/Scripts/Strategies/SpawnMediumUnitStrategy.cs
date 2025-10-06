using System;
using System.Linq;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public class SpawnMediumUnitStrategy : IDailyTaskStrategy
    {
        public event Action Completed;
        
        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;
        private readonly IConfigRepository _config;
        private readonly BattleField _battleField;
        private readonly IAgeNavigator _navigator;
        private readonly IUserContainer _userContainer;
        private readonly ITimelineNavigator _timelineNavigator;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;

        private DailyTaskPresenter _presenter;

        private readonly DailyTaskPresenter.Factory _factory;

        public SpawnMediumUnitStrategy(
            DailyTaskModel model, 
            DailyTaskView view,
            IConfigRepository config,
            IAgeNavigator navigator,
            ITimelineNavigator timelineNavigator,
            BattleField battleField,
            IUserContainer userContainer,
            DailyTaskPresenter.Factory factory)
        {
            _model = model;
            _view = view;
            _config = config;
            _navigator = navigator;
            _battleField = battleField;
            _userContainer = userContainer;
            _factory = factory;
            _timelineNavigator = timelineNavigator;
        }

        public void Initialize()
        {
            _navigator.AgeChanged += OnAgeChanged;
            _battleField.PlayerUnitSpawner.UnitSpawned += OnUnitSpawned;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            DefineTarget();
            SetAdditionalDescription();
        }

        private void OnAgeChanged()
        {
            if (!_model.IsReady)
            {
                DefineTarget();
                SetAdditionalDescription();
                _model.State.ResetProgress();
            }
        }

        public void Execute()
        {
            _presenter = _factory.Create(_model, _view);
            _presenter.Initialize();
            _presenter.CompleteClicked += OnCompleteClicked;
        }

        private void OnCompleteClicked() => Completed?.Invoke();

        public void Dispose()
        {
            _battleField.PlayerUnitSpawner.UnitSpawned -= OnUnitSpawned;
            _presenter.CompleteClicked -= OnCompleteClicked;
            _presenter?.Dispose();
            _navigator.AgeChanged -= OnAgeChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
        }

        private void OnTimelineChanged() => OnAgeChanged();

        private void OnUnitSpawned(UnitBase unit)
        {
            if (unit.Type == UnitType.Medium && !_model.IsReady)
            {
                _model.State.AddProgress(1);
            }
        }

        private void SetAdditionalDescription()
        {
            var additionalDescription = TimelineConfig
                .GetAgeRelatedWarrior(TimelineState.TimelineId, TimelineState.AgeId, UnitType.Medium).Name;
            _model.SetAdditionalDescription(additionalDescription);
        }

        private void DefineTarget()
        {
            int argument = TimelineState.GetUpgradeLevel(UpgradeItemType.FoodProduction);

            var targetFunction = _model.TargetFunctions.FirstOrDefault();
            
            if(targetFunction != null) _model.SetTarget(targetFunction.GetInt(argument));
            else
            {
                int defaultTargetValue = 1;
                _model.SetTarget(defaultTargetValue);
            }
        }
    }
}