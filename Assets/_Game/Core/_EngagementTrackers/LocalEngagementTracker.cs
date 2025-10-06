using _Game.Core._GameListenerComposite;
using _Game.Core._Time;
using _Game.Core.Services.UserContainer;

namespace _Game.Core._EngagementTrackers
{
    public class LocalEngagementTracker : ILevelChangeListener
    {
        private readonly IUserContainer _userContainer;

        public LocalEngagementTracker(IUserContainer userContainer) => 
            _userContainer = userContainer;

        void ILevelChangeListener.OnLevelChange(int level) => 
            _userContainer.State.EngagementState.AddLevelCompleted(GlobalTime.UtcNow);
    }
}