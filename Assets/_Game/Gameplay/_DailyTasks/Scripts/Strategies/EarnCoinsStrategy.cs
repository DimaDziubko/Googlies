using System;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public class EarnCoinsStrategy : IDailyTaskStrategy
    {
        public event Action Completed;
        
        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;
        
        private readonly IUserContainer _userContainer;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IAgeNavigator _ageNavigator;

        ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private DailyTaskPresenter _presenter;
        
        private readonly DailyTaskPresenter.Factory _factory;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly TemporaryCurrencyBank _temporaryBank;

        private CurrencyCell Cell => _temporaryBank.GetCell(CurrencyType.Coins);

        public EarnCoinsStrategy(
            DailyTaskModel model,
            DailyTaskView view,
            IUserContainer userContainer,
            IBattleNavigator battleNavigator,
            ITimelineNavigator timelineNavigator,
            TemporaryCurrencyBank temporaryBank,
            IAgeNavigator ageNavigator,
            DailyTaskPresenter.Factory factory)
        {
            _model = model;
            _view = view;
            _temporaryBank = temporaryBank;
            _userContainer = userContainer;
            _battleNavigator = battleNavigator;
            _ageNavigator = ageNavigator;
            _factory = factory;
            _timelineNavigator = timelineNavigator;
        }

        public void Initialize()
        {
            Cell.OnAmountAdded += OnCoinsCollected;
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
            
            var targetFunction = _model.TargetFunctions.Count - 1 <= _battleNavigator.CurrentBattleIdx 
                ? _model.TargetFunctions[_battleNavigator.CurrentBattleIdx] 
                : _model.TargetFunctions[0];
            
            if(targetFunction != null) _model.SetTarget(targetFunction.GetInt(argument));
            else
            {
                int defaultTargetValue = 1;
                _model.SetTarget(defaultTargetValue);
            }
        }

        private void OnCoinsCollected(double value)
        {
            if (!_model.IsReady)
            {
                _model.State.AddProgress((float)value);
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
            _presenter.CompleteClicked -= OnCompleteClicked;
            _presenter?.Dispose();
            Cell.OnAmountAdded -= OnCoinsCollected;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
        }

        private void OnTimelineChanged() => OnAgeChanged();
    }
}