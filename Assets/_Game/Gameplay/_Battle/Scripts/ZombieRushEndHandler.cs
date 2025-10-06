using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._Dungeons.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._Battle.Scripts
{
    public class DungeonResultPopupController
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly IConfigRepository _config;

        public DungeonResultPopupController(
            TemporaryCurrencyBank temporaryBank,
            IWorldCameraService cameraService, 
            IAudioService audioService,
            IConfigRepository config)
        {
            _config = config;
            _cameraService = cameraService;
            _audioService = audioService;
            _temporaryBank = temporaryBank;
        }

        public async UniTask ShowPopup(IDungeonModel dungeonModel)
        {
            var presenter = new DungeonResultPopupPresenter( _temporaryBank, _config.IconConfigRepository, dungeonModel);
            var popupProvider = new DungeonResultPopupProvider(_cameraService, _audioService, presenter);

            var popup = await popupProvider.Load();
            await popup.Value.AwaitForDecision();

            popup.Dispose();
        }
    }
}