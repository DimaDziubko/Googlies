using System;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class ForcedUpdateService : LocalAssetLoader, IDisposable
    {
        private Disposable<ForcedUpdatePopup> _popup;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        public ForcedUpdateService(
            IWorldCameraService cameraService,
            IAudioService audioService
        )
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }

        public async void Initialize(bool isShow)
        {
            if (isShow)
                await Load(UpdateState.ForceUpdate);
        }

        public void Dispose()
        {
            if (_popup != null)
            {
                _popup.Value.Cleanup();
                _popup.Dispose();
                _popup = null;
            }
        }

        public async UniTask<Disposable<ForcedUpdatePopup>> Load(UpdateState updateState)
        {
            if (_popup != null) return _popup;

            _popup = await LoadDisposable<ForcedUpdatePopup>(AssetsConstants.FORCEDUPDATE_POPUP, Constants.Scenes.UI);
            _popup.Value.Construct(
                _cameraService,
                _audioService,
                this,
                updateState
            );

            return _popup;
        }
        
        public void UpdateApp() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
    }
}