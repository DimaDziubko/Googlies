using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.AssetManagement;
using _Game.Core.Navigation.Age;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI._TimelineInfoPresenter;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineInfoScreenProvider : LocalAssetLoader, ITimelineInfoScreenProvider
    {
        private readonly IAudioService _audioService;
        private readonly ITimelineInfoPresenter _timelineInfoPresenter;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;
        private readonly IAdsService _adsService;
        private readonly IAgeNavigator _ageNavigator;

        private Disposable<TimelineInfoScreen> _screen;

        public TimelineInfoScreenProvider(
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAdsService adsService, 
            IAgeNavigator ageNavigator)
        {
            _audioService = audioService;
            _timelineInfoPresenter = timelineInfoPresenter;
            _logger = logger;
            _cameraService = cameraService;
            _adsService = adsService;
            _ageNavigator = ageNavigator;
        }
        public async UniTask<Disposable<TimelineInfoScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await
                LoadDisposable<TimelineInfoScreen>(AssetsConstants.TIMELINE_INFO_SCREEN, Constants.Scenes.UI);
            
            _screen.Value.Construct(
                _audioService,
                _timelineInfoPresenter,
                _logger,
                _cameraService,
                _adsService,
                _ageNavigator);
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