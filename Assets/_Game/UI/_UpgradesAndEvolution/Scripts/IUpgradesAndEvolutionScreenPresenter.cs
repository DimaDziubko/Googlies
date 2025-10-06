namespace _Game.UI._UpgradesAndEvolution.Scripts
{
    public interface IUpgradesAndEvolutionScreenPresenter
    {
        UpgradeAndEvolutionScreen Screen { set; }
        void OnScreenActiveChanged(bool isActive);
        void OnUpgradesAndEvolutionScreenOpened();
        void OnUpgradesAndEvolutionScreenClosed();
        void OnScreeDispose();
    }
}