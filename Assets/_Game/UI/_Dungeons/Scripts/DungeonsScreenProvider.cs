using System.Threading;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Dungeons.Scripts
{
    public interface IDungeonsScreenProvider
    {
        UniTask<Disposable<DungeonsScreen>> Load();
        void Dispose();
    }
    
    public class DungeonsScreenProvider  : LocalAssetLoader, IDungeonsScreenProvider 
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IDungeonsScreenPresenter _presenter;

        private Disposable<DungeonsScreen> _screen;

        public DungeonsScreenProvider(
            IWorldCameraService cameraService, 
            IDungeonsScreenPresenter presenter)
        {
            _cameraService = cameraService;
            _presenter = presenter;
        }
        
        public async UniTask<Disposable<DungeonsScreen>> Load()
        {
            if (_screen != null) return _screen;

            _screen = await LoadDisposable<DungeonsScreen>(AssetsConstants.DUNGEONS_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _presenter);
            return _screen;
        }

        public void Dispose()
        {
            if (_screen != null)
            {
                _screen.Value.Dispose();
                _screen.Dispose();
                _screen = null;
            }
        }
    }
}