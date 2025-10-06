using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._ParticleAttractorSystem;
using Sirenix.OdinInspector;

namespace _Game.LiveopsCore
{
    public abstract class GameEventPresenterBase
    {
        [ShowInInspector, ReadOnly]
        protected GameEventView _view;

        protected readonly IMyLogger _logger;
        protected readonly IWorldCameraService _cameraService;
        protected readonly IAudioService _audioService;
        protected readonly CurrencyBank _bank;
        protected readonly IUserContainer _userContainer;
        protected readonly IParticleAttractorRegistry _particleAttractorRegistry;
        protected readonly IIconConfigRepository _iconConfig;
        protected readonly IAPProvider _provider;

        protected GameEventPresenterBase(
            GameEventView view,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAudioService audioService,
            CurrencyBank bank,
            IUserContainer userContainer,
            IIconConfigRepository iconConfig,
            IParticleAttractorRegistry particleAttractorRegistry,
            IAPProvider provider)
        {
            _view = view;
            _logger = logger;
            _cameraService = cameraService;
            _audioService = audioService;
            _bank = bank;
            _userContainer = userContainer;
            _iconConfig = iconConfig;
            _particleAttractorRegistry = particleAttractorRegistry;
            _provider = provider;
        }

        public GameEventView View => _view;

        public abstract GameEventBase Model { get; }

        public void Initialize()
        {
            SetupIcon();
            OnInitialize();
            Model.ShowcaseToken.Requested += OnShowRequested;
        }

        public void Dispose()
        {
            OnDispose();
            Model.ShowcaseToken.Requested -= OnShowRequested;
        }

        private void SetupIcon() => _view.SetIcon(Model.Icon);

        protected abstract void OnInitialize();
        protected abstract void OnShowRequested();
        protected virtual void OnDispose() { }

        public void SetView(GameEventView view)
        {
            _view = view;
        }
        public abstract void SetGameEvent(GameEventBase gameEvent);
    }
}