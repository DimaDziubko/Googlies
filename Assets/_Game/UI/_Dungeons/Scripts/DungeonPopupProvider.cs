using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Dungeons.Scripts
{
    public interface IDungeonPopupProvider
    {
        UniTask<Disposable<DungeonPopup>> Load();
    }

    public class DungeonPopupProvider : LocalAssetLoader, IDungeonPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly DungeonPopupPresenter _presenter;
        private readonly IAudioService _audioService;

        public DungeonPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            DungeonPopupPresenter presenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _presenter = presenter;
        }
        
        public async UniTask<Disposable<DungeonPopup>> Load()
        {
            var popup = await LoadDisposable<DungeonPopup>(AssetsConstants.DUNGEON_POPUP, Constants.Scenes.UI);
            
            popup.Value.Construct(
                _cameraService,
                _audioService,
                _presenter);
            
            return popup;
        }
    }
}