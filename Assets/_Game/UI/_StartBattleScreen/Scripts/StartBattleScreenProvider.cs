using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._StartBattleScreen.Scripts
{
    public  class StartBattleScreenProvider : LocalAssetLoader, IStartBattleScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;
        private readonly IStartBattleScreenPresenter _presenter;

        private Disposable<StartBattleScreen> _screen;

        public StartBattleScreenProvider(
            IWorldCameraService cameraService,
            IMyLogger logger,
            IStartBattleScreenPresenter presenter)
        {
            _cameraService = cameraService;
            _logger = logger;
            _presenter = presenter;
        }
        public async UniTask<Disposable<StartBattleScreen>> Load()
        {
            if (_screen != null) return _screen;
            _screen = await
                LoadDisposable<StartBattleScreen>(AssetsConstants.START_BATTLE_SCREEN, Constants.Scenes.UI);
            
            _screen.Value.Construct(
                _cameraService.UICameraOverlay,
                _logger,
                _presenter);
            return _screen;
        }
        
        public void Dispose()
        {
            if (_screen != null)
            {
                _logger.Log("StartBattleScreenProvider unloaded", DebugStatus.Info);
                
                _screen.Value.Dispose();
                _screen.Dispose();
                _screen = null;
            }
        }
        
        public StartBattleScreen GetScreen() => 
            _screen?.Value.OrNull();
    }
}