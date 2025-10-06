using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.UI.Settings.Scripts;
using Zenject;

namespace _Game.UI._Header.Scripts
{
    public class HeaderPresenter : IInitializable, IGameScreenListener<IGameScreenWithInfo>
    {
        private readonly Header _header;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IAudioService _audioService;

        public HeaderPresenter(
            IWorldCameraService cameraService,
            Header header,
            ISettingsPopupProvider settingsPopupProvider,
            IAudioService audioService,
            IMyLogger logger
            )
        {
            _header = header;
            _logger = logger;
            _cameraService = cameraService;
            _settingsPopupProvider = settingsPopupProvider;
            _audioService = audioService;
        }

        public void Initialize() => _header.SetCamera(_cameraService.UICameraOverlay);

        void IGameScreenListener<IGameScreenWithInfo>.OnScreenOpened(IGameScreenWithInfo screen)
        {
            _header.SetInfo(screen.Info);
            Subscribe();
        }
        void IGameScreenListener<IGameScreenWithInfo>.OnInfoChanged(IGameScreenWithInfo screen) => _header.SetInfo(screen.Info);
        void IGameScreenListener<IGameScreenWithInfo>.OnRequiresAttention(IGameScreenWithInfo screen) { }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenClosed(IGameScreenWithInfo screen) { UnSubscribe(); }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenActiveChanged(IGameScreenWithInfo screen, bool isActive) { }
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenDisposed(IGameScreenWithInfo screen) { UnSubscribe(); }

        private void Subscribe()
        {
            _logger.Log("[HeaderPresenter] Subscribe");
            _header.SettingsClicked += OnSettingsClicked;
        }

        private void UnSubscribe()
        {
            _logger.Log("[HeaderPresenter] UnSubscribe");
            _header.SettingsClicked -= OnSettingsClicked;

        }

        private async void OnSettingsClicked()
        {
            PlayButtonSound();
            var popup = await _settingsPopupProvider.Load();
            await popup.Value.AwaitForExit();
            popup.Dispose();
        }
        private void PlayButtonSound() => _audioService.PlayButtonSound();

    }
}