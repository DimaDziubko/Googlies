using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Core.Loading
{
    public enum UpdateState
    {
        Invalid,
        UpToDate,
        OptionalUpdate,
        ForceUpdate
    }
    public class ForcedUpdatePopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _buttonUpdate;
        [SerializeField] private Button _buttonCancel;
        [Space]
        [SerializeField] private PopupAppearanceAnimation _animation;


        private IAudioService _audioService;
        private ForcedUpdateService _forcedUpdateService;
        private UpdateState _updateState;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Construct(
            IWorldCameraService cameraService,
            IAudioService audioService,
            ForcedUpdateService forcedUpdateService,
            UpdateState updateState
        )
        {
            _audioService = audioService;
            //_canvas.worldCamera = cameraService.UICameraOverlay;
            _forcedUpdateService = forcedUpdateService;
            _updateState = updateState;

            Initialize();
            Subscribe();
            _animation.PlayShow();
        }

        public void Cleanup()
        {
            Unsubscribe();
        }
        private void Initialize()
        {
            switch (_updateState)
            {
                case UpdateState.ForceUpdate:


                    Debug.Log("ForcedUpdateInfo Force update required. Showing update popup.");
                    _buttonCancel.gameObject.SetActive(false);
                    break;

                case UpdateState.OptionalUpdate:

                    Debug.Log("ForcedUpdateInfo Optional update available. Showing update popup with skip option.");
                    break;

                case UpdateState.UpToDate:

                    Debug.Log("ForcedUpdateInfo Application is up to date.");
                    break;

                default:

                    Debug.LogError("ForcedUpdateInfo Invalid update state.");
                    break;
            }
        }
        private void Subscribe()
        {
            _buttonUpdate.onClick.AddListener(OpUpdateApp);
            _buttonCancel.onClick.AddListener(OnCancelled);
        }
        private void Unsubscribe()
        {
            _buttonUpdate.onClick.RemoveAllListeners();
            _buttonCancel.onClick.RemoveAllListeners();
        }

        private void OnCancelled()
        {
            PlayButtonSound();
            _animation.PlayHide(() => _forcedUpdateService.Dispose());
        }
        private void OpUpdateApp()
        {
            PlayButtonSound();
            _forcedUpdateService.UpdateApp();
        }
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}