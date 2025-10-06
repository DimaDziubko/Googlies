using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Skills.Scripts
{
    public interface ISkillsScreenProvider
    {
        UniTask<Disposable<SkillsScreen>> Load();
        void Dispose();
    }

    public class SkillsScreenProvider : LocalAssetLoader, ISkillsScreenProvider

    { 
        private readonly IWorldCameraService _cameraService;
        private readonly ISkillsScreenPresenter _skillsScreenPresenter;
        private readonly IMyLogger _logger;

        private Disposable<SkillsScreen> _screen;

        public SkillsScreenProvider(
            IWorldCameraService cameraService,
            ISkillsScreenPresenter skillsScreenPresenter,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _skillsScreenPresenter = skillsScreenPresenter;
            _logger = logger;
        }

        public async UniTask<Disposable<SkillsScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await LoadDisposable<SkillsScreen>(AssetsConstants.SKILLS_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _skillsScreenPresenter,
                _logger);
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