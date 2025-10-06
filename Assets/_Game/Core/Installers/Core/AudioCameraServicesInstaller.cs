using _Game.Audio.Scripts;
using _Game.Core.Prefabs;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class AudioCameraServicesInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uICameraOverlay;
        [SerializeField] private SFXSourcesHolder _sfxSourcesHolder;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SoundsHolder _soundsHolder;
        [SerializeField] private SoundService _soundService;

        public override void InstallBindings()
        {
            BindCameraService();
            BindAudioService();
            BindSoundService();
        }

        private void BindCameraService()
        {
            Container
                .Bind<IWorldCameraService>()
                .To<WorldCameraService>()
                .AsSingle()
                .WithArguments(_mainCamera, _uICameraOverlay);
        }

        private void BindAudioService()
        {
            Container
                .Bind<IAudioService>()
                .To<AudioService>()
                .AsSingle()
                .WithArguments(_audioMixer, _sfxSourcesHolder, _musicSource, _soundsHolder);
        }

        private void BindSoundService()
        {
            Container
                .BindInterfacesAndSelfTo<SoundService>()
                .FromInstance(_soundService)
                .AsSingle()
                .NonLazy(); 
        }
    }
}