namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialManager
    {
        public bool Register(ITutorialStep tutorialStep);
        public void UnRegister(ITutorialStep tutorialStep);
    }
}