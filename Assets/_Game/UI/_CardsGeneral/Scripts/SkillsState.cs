using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.UI._MainMenu.State;
using _Game.UI._Skills.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class SkillsState : ILocalState
    {
        private readonly ISkillsScreenProvider _provider;
        private readonly IMyLogger _logger;

        private readonly GeneralCardsScreenPresenter _presenter;

        private Disposable<SkillsScreen> _skillsScreen;

        public SkillsState(
            ISkillsScreenProvider provider,
            GeneralCardsScreenPresenter presenter,
            IMyLogger logger)
        {
            _provider = provider;
            _presenter = presenter;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _skillsScreen = await _provider.Load();
        
        public void SetActive(bool isActive)
        {
            if (_skillsScreen != null) 
                _skillsScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            _presenter.HighlightSkillsBtn();
            
            _presenter.QuickBoostInfoPresenter.SetMainSource(BoostSource.Skills);
            _presenter.QuickBoostInfoPresenter.SetSubSource(BoostSource.Total);

            if (_skillsScreen != null)
            {
                _skillsScreen.Value.Show();
                _presenter.CompleteSkillsStep();
            }
        }

        public void Exit()
        {
            if (_skillsScreen?.Value.OrNull() != null)
            {
                _skillsScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but skillsScreen is null", DebugStatus.Warning);
            }
            _presenter.UnHighlightSkillsBtn();
            _presenter.TryToShowSkillsStep();
        }

        public void Cleanup() => _provider.Dispose();
    }
}