using _Game.Core._Logger;
using _Game.UI._MainMenu.State;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._TravelScreen.Scripts
{
    public class TravelState : ILocalState
    {
        private readonly ITravelScreenProvider _provider;
        private readonly IMyLogger _logger;

        private Disposable<TravelScreen> _travelScreen; 
        
        public TravelState(
            ITravelScreenProvider provider,
            IMyLogger logger)
        {
            _provider = provider;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _travelScreen = await _provider.Load();
        
        public void SetActive(bool isActive)
        {
            if (_travelScreen != null) 
                _travelScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            if (_travelScreen != null)
            {
                _travelScreen.Value.Show();
            }
        }

        public void Exit()
        {
            if (_travelScreen?.Value.OrNull() != null)
            {
                _travelScreen?.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _travelScreen is null", DebugStatus.Warning);
            }
        }

        public void Cleanup() => _provider.Dispose();
    }
}