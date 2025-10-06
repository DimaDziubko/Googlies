using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;

namespace _Game.Core.Services.Analytics
{
    public class TutorialTracker
    {
        private readonly IUserContainer _userContainer;
        private readonly IAnalyticsEventSender _sender;

        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;
        public TutorialTracker(
            IUserContainer userContainer,
            IAnalyticsEventSender sender)
        {
            _userContainer = userContainer;
            _sender = sender;
        }

        public void Initialize()
        {
            TutorialState.StepsCompletedChanged += OnStepCompleted;
        }

        public void Dispose()
        {
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
        }
        
        private void OnStepCompleted(int step)
        {
            _sender.Tutorial(step);
        }
    }
}