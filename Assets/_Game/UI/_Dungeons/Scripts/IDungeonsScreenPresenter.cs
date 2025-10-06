namespace _Game.UI._Dungeons.Scripts
{
    public interface IDungeonsScreenPresenter
    {
        DungeonsScreen Screen { set; }
        void OnScreenOpened();
        void OnScreenClosed();
        void OnScreeDispose();
        void OnScreeActiveChanged(bool isActive);
    }
}