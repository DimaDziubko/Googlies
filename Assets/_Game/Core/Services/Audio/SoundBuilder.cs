using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Services.Audio
{
    public class SoundBuilder
    {
        private readonly ISoundService _soundService;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;

        public SoundBuilder(ISoundService soundService) => 
            _soundService = soundService;

        public SoundBuilder WithSoundData(SoundData soundData)
        {
            _soundData = soundData;
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }
    
        public SoundBuilder WithRandomPitch()
        {
            _randomPitch = true;
            return this;
        }
        public SoundEmitter Play()
        {
            if (!_soundService.CanPlaySound(_soundData)) return null;

            SoundEmitter soundEmitter = _soundService.Get(_soundData.Clip);
            if (soundEmitter == null) return null;
            soundEmitter.Initialize(_soundData);
            soundEmitter.Transform.position = _position;
            soundEmitter.Transform.parent = _soundService.Transform;

            if (_randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (_soundData.FrequentSound)
            {
                if (!_soundService.FrequentSoundEmitters.ContainsKey(_soundData.Clip))
                {
                    _soundService.FrequentSoundEmitters[_soundData.Clip] = new Queue<SoundEmitter>();
                }

                _soundService.FrequentSoundEmitters[_soundData.Clip].Enqueue(soundEmitter);
            }

            if (_soundData.Loop)
            {
                soundEmitter.Play();
            }
            else
            {
                soundEmitter.PlayOneShot();
            }
            
            return soundEmitter;
        }

    }
}