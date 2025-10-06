using _Game.Core._IconContainer;
using _Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.Gameplay._Battle.Scripts
{
    public class AmbienceController
    {
        private AudioClip _ambience;
        
        private readonly IAudioService _audioService;
        private readonly AmbienceContainer _container;

        public AmbienceController(
            IAudioService audioService,
            AmbienceContainer container)
        {
            _audioService = audioService;
            _container = container;
        }
        
        public void PlayAmbience()
        {
            if (_audioService != null && _ambience != null)
            {
                _audioService.Play(_ambience);
            }
        }

        public void SetAmbience(string ambienceKey) => 
            _ambience = _container.Get(ambienceKey);


        public void StopAmbience()
        {
            if (_audioService != null && _ambience != null)
            {
                _audioService.Stop();
            }
        }
    }
}