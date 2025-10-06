using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Skills.Scripts
{
    public interface ISkillPopupProvider
    {
        UniTask<Disposable<SkillPopup>> Load();
    }
    
    public class SkillPopupProvider : LocalAssetLoader, ISkillPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IIconConfigRepository _config;
        private readonly BoostContainer _boosts;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private readonly SkillPopupPresenter _skillPopupPresenter;

        public SkillPopupProvider(
            SkillPopupPresenter skillPopupPresenter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _skillPopupPresenter = skillPopupPresenter;
            _cameraService = cameraService;
            _audioService = audioService;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            _userContainer = userContainer;
        }
        
        public async UniTask<Disposable<SkillPopup>> Load()
        {
            var popup = await LoadDisposable<SkillPopup>(AssetsConstants.SKILL_POPUP, Constants.Scenes.UI);
            popup.Value.Construct(
                _skillPopupPresenter, 
                _cameraService,
                _audioService,
                _config,
                _boosts,
               _userContainer,
                _logger);

            return popup;
        }
    }
}