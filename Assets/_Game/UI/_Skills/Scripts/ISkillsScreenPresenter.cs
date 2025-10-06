namespace _Game.UI._Skills.Scripts
{
    public interface ISkillsScreenPresenter
    {
        SkillsScreen Screen { get; set; }
        void OnSkillScreenOpened();
        void OnSkillScreenClosed();
        void OnSkillScreenDisposed();
    }
}