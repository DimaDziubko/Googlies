using System;
using System.Linq;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public class ProduceFoodStrategy : IDailyTaskStrategy
    {
        public event Action Completed;
        
        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;
        
        private readonly FoodGenerator _foodGenerator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IUserContainer _userContainer;

        ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private DailyTaskPresenter _presenter;
        
        private readonly DailyTaskPresenter.Factory _factory;
        private readonly ITimelineNavigator _timelineNavigator;

        public ProduceFoodStrategy(
            DailyTaskModel model,
            DailyTaskView view,
            FoodGenerator foodGenerator,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            IUserContainer userContainer,
            DailyTaskPresenter.Factory factory)
        {
            _model = model;
            _view = view;
            _foodGenerator = foodGenerator;
            _ageNavigator = ageNavigator;
            _userContainer = userContainer;
            _factory = factory;
            _timelineNavigator = timelineNavigator;
        }

        public void Initialize()
        {
            _foodGenerator.FoodProduced += OnFoodProduced;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            DefineTarget();
        }

        private void OnAgeChanged()
        {
            if (!_model.IsReady)
            {
                DefineTarget();
                _model.State.ResetProgress();
            }
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

        public void Execute()
        {
            _presenter = _factory.Create(_model, _view);
            _presenter.Initialize();
            _presenter.CompleteClicked += OnCompleteClicked;
        }

        public void Dispose()
        {
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _presenter.CompleteClicked -= OnCompleteClicked;
            _presenter?.Dispose();
            _foodGenerator.FoodProduced -= OnFoodProduced;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
        }

        private void OnTimelineChanged() => OnAgeChanged();

        private void OnCompleteClicked() => Completed?.Invoke();

        private void OnFoodProduced(int value)
        {
            if (!_model.IsReady)
            {
                _model.State.AddProgress(value);
            }
        }
    }
}