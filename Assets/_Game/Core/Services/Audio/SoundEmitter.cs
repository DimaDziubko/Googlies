using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Services.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        private ISoundService _soundService;
        public SoundData Data { get; private set; }


        [ShowInInspector, ReadOnly]
        private Coroutine _playingCoroutine;

        private bool _isPaused;

        [SerializeField] private Transform _transform;
        [SerializeField] private AudioSource _audioSource;

        public bool IsPlaying => _audioSource.isPlaying;
        public bool IsPaused => _isPaused;
        public Transform Transform => _transform;
        public void Construct(
            ISoundService soundService)
        {
            _soundService = soundService;
        }

        public void Initialize(SoundData data)
        {
            Data = data;
            _audioSource.clip = data.Clip;
            _audioSource.outputAudioMixerGroup = data.AudioMixerGroup;
            _audioSource.loop = data.Loop;
            _audioSource.playOnAwake = data.PlayOnAwake;
            
            _audioSource.mute = data.Mute;
            _audioSource.bypassEffects = data.BypassEffects;
            _audioSource.bypassListenerEffects = data.BypassListenerEffects;
            _audioSource.bypassReverbZones = data.BypassReverbZones;
            
            _audioSource.priority = data.Priority;
            _audioSource.volume = data.Volume;
            _audioSource.pitch = data.Pitch;
            _audioSource.panStereo = data.PanStereo;
            _audioSource.spatialBlend = data.SpatialBlend;
            _audioSource.reverbZoneMix = data.ReverbZoneMix;
            _audioSource.dopplerLevel = data.DopplerLevel;
            _audioSource.spread = data.Spread;
            
            _audioSource.minDistance = data.MinDistance;
            _audioSource.maxDistance = data.MaxDistance;
            
            _audioSource.ignoreListenerVolume = data.IgnoreListenerVolume;
            _audioSource.ignoreListenerPause = data.IgnoreListenerPause;
            
            _audioSource.rolloffMode = data.RolloffMode;
        }
        
        public void PlayOneShot() {
            if (_playingCoroutine != null) {
                StopCoroutine(_playingCoroutine);
            }
            
            Play();
            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Play()
        {
            _audioSource.Play();
        }

        public void SetPaused(bool isPaused)
        {
            if (isPaused)
            {
                _isPaused = true;
                _audioSource.Pause();
            }
            else
            {
                _isPaused = false;
                _audioSource.UnPause();
            }
        }
        
        IEnumerator WaitForSoundToEnd() 
        {
            yield return new WaitWhile(() => IsPlaying);
            Stop();
        }

        public void Stop() 
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            _soundService.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) => 
            _audioSource.pitch += UnityEngine.Random.Range(min, max);
    }
}