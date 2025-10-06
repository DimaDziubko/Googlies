namespace _Game.UI._UpgradesScreen.Scripts
{
    public interface IUpgradesScreenPresenter
    {
        UpgradesScreen Screen { get; set; }
        void OnUpgradesScreenOpened();
        void OnUpgradesScreenClosed();
        void OnUpgradesScreenDisposed();
        void OnScreenActiveChanged(bool isActive);
    }
}