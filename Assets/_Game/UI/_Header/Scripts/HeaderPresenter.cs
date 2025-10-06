using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Zenject;

namespace _Game.UI._Header.Scripts
{
    public class HeaderPresenter : IInitializable, IGameScreenListener<IGameScreenWithInfo>
    {
        private readonly Header _header;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;

        public HeaderPresenter(
            IWorldCameraService cameraService,
            Header header,
            IMyLogger logger)
        {
            _header = header;
            _logger = logger;
            _cameraService = cameraService;
        }

        public void Initialize() => _header.SetCamera(_cameraService.UICameraOverlay);
        
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenOpened(IGameScreenWithInfo screen) => _header.SetInfo(screen.Info);
        void IGameScreenListener<IGameScreenWithInfo>.OnInfoChanged(IGameScreenWithInfo screen) => _header.SetInfo(screen.Info);
        void IGameScreenListener<IGameScreenWithInfo>.OnRequiresAttention(IGameScreenWithInfo screen) { }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenClosed(IGameScreenWithInfo screen) { }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenActiveChanged(IGameScreenWithInfo screen, bool isActive) { }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenDisposed(IGameScreenWithInfo screen) { }
    }
}