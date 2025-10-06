using System;
using System.Linq;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Hud._DailyTaskView;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public class AdsWatchStrategy : IDailyTaskStrategy
    {
        public event Action Completed;
        
        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;
        
        private readonly IUserContainer _userContainer;

        private readonly DailyTaskPresenter.Factory _factory;
        private readonly ITimelineNavigator _timelineNavigator;
        
        private DailyTaskPresenter _presenter;
        private IAdsStatisticsReadonly AdsStatistics => _userContainer.State.AdsStatistics;
        private IAdsWeeklyWatchStateReadonly AdsWeeklyWatchState => _userContainer.State.AdsWeeklyWatchState;

        public AdsWatchStrategy(
            DailyTaskModel model, 
            DailyTaskView view,
            IUserContainer userContainer,
            ITimelineNavigator timelineNavigator,
            DailyTaskPresenter.Factory factory)
        {
            _model = model;
            _view = view;
            _userContainer = userContainer;
            _factory = factory;
            _timelineNavigator = timelineNavigator;
        }

        public void Initialize()
        {
            AdsStatistics.AdsReviewedChanged += OnAdsReviewedChanged;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            DefineTarget();
        }

        private void DefineTarget()
        {
            int argument = AdsWeeklyWatchState.LastWeekWatchedAds;
            int minArgumentValue = 2;
            argument = argument < minArgumentValue ? minArgumentValue : argument;

            var targetFunction = _model.TargetFunctions.FirstOrDefault();
            if(targetFunction != null) _model.SetTarget(targetFunction.GetInt(argument));
            else
            {
                int defaultTargetValue = 1;
                _model.SetTarget(defaultTargetValue);
            }
        }

        private void OnTimelineChanged()
        {
            if (!_model.IsReady)
            {
                DefineTarget();
                _model.State.ResetProgress();
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
            _presenter.CompleteClicked -= OnCompleteClicked;
            _presenter?.Dispose();
            AdsStatistics.AdsReviewedChanged -= OnAdsReviewedChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
        }

        private void OnCompleteClicked() => Completed?.Invoke();

        private void OnAdsReviewedChanged() => _model.State.AddProgress(1);
    }
}