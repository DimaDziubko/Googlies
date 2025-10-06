using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._Tutorial
{
    public class TutorialStateHandler : ITutorialStateHandler
    {
        private readonly IUserContainer _userContainer;

        public TutorialStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void CompleteTutorialStep(int step)
        {
            _userContainer.State.TutorialState.AddCompletedStep(step);
            _userContainer.RequestSaveGame();
        }
    }
}