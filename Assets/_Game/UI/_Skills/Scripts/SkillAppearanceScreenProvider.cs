using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Skills.Scripts
{
    public interface ISkillAppearanceScreenProvider
    {
        UniTask<Disposable<SkillAppearanceScreen>> Load();
        void Dispose();
    }
    public class SkillAppearanceScreenProvider : LocalAssetLoader, ISkillAppearanceScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IUserContainer _userContainer;

        private Disposable<SkillAppearanceScreen> _screen;

        public SkillAppearanceScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IUserContainer userContainer)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _userContainer = userContainer;
        }

        public async UniTask<Disposable<SkillAppearanceScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await LoadDisposable<SkillAppearanceScreen>(AssetsConstants.SKILL_APPEARANCE_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _userContainer); 
            return _screen;
        }
        public void Dispose()
        {
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }
    }
}