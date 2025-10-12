using _Game.Core._Logger;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.UI._MainMenu.State;
using _Game.UI._TravelScreen.Scripts;
using _Game.UI._UpgradesAndEvolution.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolutionState : ILocalState
    {
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IEvolveScreenProvider _evolveProvider;
        private readonly ITravelScreenProvider _travelProvider;
        private readonly IMyLogger _logger;

        private readonly LocalStateMachine _subStateMachine;
        private readonly UpgradesAndEvolutionScreenPresenter _presenter;
 
        public EvolutionState(
            IEvolveScreenProvider evolveProvider,
            ITravelScreenProvider travelProvider,
            UpgradesAndEvolutionScreenPresenter presenter,
            IMyLogger logger,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator)
        {
            _subStateMachine = new LocalStateMachine();
            _evolveProvider = evolveProvider;
            _travelProvider = travelProvider;
            _presenter = presenter;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
        }
        
        public async UniTask InitializeAsync()
        {
            _subStateMachine.AddState(new TravelState(_travelProvider, _logger));
            
            await _subStateMachine.InitializeAsync();
        }

        public void SetActive(bool isActive) => 
            _subStateMachine.SetActive(isActive);

        public void Enter()
        {
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            
            // _presenter.HighlightEvolutionBtn(); //TODO

            OnTimelineChanged();
        }

        public void Exit()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            
            // _presenter.UnHighlightEvolutionBtn(); //TODO
            _subStateMachine.Exit();
        }

        private void OnAgeChanged()
        {
            _subStateMachine.Exit();
            
            if(!_ageNavigator.IsNextAge() || _ageNavigator.IsAllBattlesWon()) _subStateMachine.Enter<TravelState>();
            else
            {
                //_subStateMachine.Enter<EvolveState>(); //TODO
            }
        }

        private void OnTimelineChanged() => OnAgeChanged();

        public void Cleanup()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _subStateMachine.Cleanup();
        }
    }
}