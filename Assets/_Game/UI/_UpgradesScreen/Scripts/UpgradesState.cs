using _Game.Core._Logger;
using _Game.UI._MainMenu.State;
using _Game.UI._UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UpgradesState : ILocalState
    {
        private readonly IUpgradesScreenProvider _provider;
        private readonly IMyLogger _logger;

        private readonly UpgradesAndEvolutionScreenPresenter _presenter;
        
        private Disposable<UpgradesScreen> _upgradesScreen;

        public UpgradesState(
            IUpgradesScreenProvider provider,
            UpgradesAndEvolutionScreenPresenter presenter,
            IMyLogger logger)
        {
            _provider = provider;
            _presenter = presenter;
            _logger = logger;
        }

        public async UniTask InitializeAsync() => _upgradesScreen = await _provider.Load();

        public void SetActive(bool isActive)
        {
            if (_upgradesScreen != null) 
                _upgradesScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            _presenter.HighlightUpgradesBtn();

            if (_upgradesScreen != null)
            {
                _upgradesScreen.Value.Show();
            }
        }

        public void Exit()
        {
            _upgradesScreen?.Value.Hide();
            _presenter.UnHighlightUpgradesBtn();
        }

        public void Cleanup() => _provider.Dispose();
    }
}