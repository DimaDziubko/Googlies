using _Game.Core._Logger;
using _Game.UI._MainMenu.State;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveState : ILocalState
    {
        private readonly IEvolveScreenProvider _provider;
        private readonly IMyLogger _logger;

        private Disposable<EvolveScreen> _evolveScreen;

        public EvolveState(
            IEvolveScreenProvider provider,
            IMyLogger logger)
        {
            _provider = provider;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _evolveScreen = await _provider.Load();
        
        public void SetActive(bool isActive)
        {
            if (_evolveScreen != null) 
                _evolveScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            if (_evolveScreen != null)
            {
                _evolveScreen.Value.Show();
            }
        }

        public void Exit()
        {
            if (_evolveScreen?.Value.OrNull() != null)
            {
                _evolveScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _evolveScreen is null", DebugStatus.Warning);
            }
        }

        public void Cleanup() => _provider.Dispose();
    }
}