using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._ParticleAttractorSystem;

namespace _Game.LiveopsCore
{
    public abstract class GameEventPresenter<TEvent> : GameEventPresenterBase
        where TEvent : GameEventBase
    {
        protected TEvent _event;

        protected GameEventPresenter(
            TEvent gameEvent,
            GameEventView view,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAudioService audioService,
            CurrencyBank bank,
            IUserContainer userContainer,
            IIconConfigRepository iconConfig,
            IParticleAttractorRegistry particleAttractorRegistry,
            IAPProvider provider)
            : base(view, logger, cameraService, audioService, bank, userContainer, iconConfig, particleAttractorRegistry, provider)
        {
            _event = gameEvent;
        }

        public override GameEventBase Model => _event;
        
        public override void SetGameEvent(GameEventBase gameEvent)
        {
            if (gameEvent is TEvent typedEvent)
            {
                _event = typedEvent;
            }
            else
            {
                _logger.Log($"Warning: Expected event of type {typeof(TEvent).Name}, but got {gameEvent.GetType().Name}", DebugStatus.Warning);
                
                _event = null;
            }
        }
    }
}