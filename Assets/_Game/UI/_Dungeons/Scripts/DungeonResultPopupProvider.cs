using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Dungeons.Scripts
{
    public interface IDungeonResultPopupProvider
    {
        UniTask<Disposable<DungeonResultPopup>> Load();
    }
    
    public class DungeonResultPopupProvider : LocalAssetLoader, IDungeonResultPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly DungeonResultPopupPresenter _presenter;
        
        public DungeonResultPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            DungeonResultPopupPresenter presenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _presenter = presenter;
        }
        
        public async UniTask<Disposable<DungeonResultPopup>> Load()
        {
            var popup = await LoadDisposable<DungeonResultPopup>(AssetsConstants.DUNGEON_RESULT_POPUP, Constants.Scenes.UI);
            
            popup.Value.Construct(
                _cameraService,
                _audioService,
                _presenter);
            
            return popup;
        }
    }
}