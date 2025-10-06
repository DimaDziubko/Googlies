namespace _Game.UI._StartBattleScreen.Scripts
{
    public interface IStartBattleScreenPresenter
    {
        StartBattleScreen Screen { set; }
        void OnScreenOpened();
        void OnScreenClosed();
        void OnScreenDispose();
        void OnActiveChanged(bool isActive);
    }
}