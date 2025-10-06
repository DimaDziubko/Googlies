using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _sFXSources;
        private readonly AudioSource _musicSource;

        private int _freeSource;

        public void PlayOneShot(AudioClip audioClip)
        {
            if (_freeSource >= _sFXSources.Length - 1)
            {
                _freeSource = 0;
            }
            
            _sFXSources[_freeSource].PlayOneShot(audioClip);

            _freeSource += 1;
        }
    }
}
