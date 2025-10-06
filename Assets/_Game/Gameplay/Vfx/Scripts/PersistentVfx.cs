using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class PersistentVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        private bool _wasEmittingBeforePause;
        
        private void Awake()
        {
            _particleSystem.Stop();
        }

        //AnimationEvent
        public void ActivateVfx()
        {
            if(_particleSystem.isEmitting) return;
            _particleSystem.Play();
        }

        //AnimationEvent
        public void DeactivateVfx() => 
            _particleSystem.Stop();

        public void SetPaused(in bool isPaused)
        {
            if (isPaused)
            {
                _wasEmittingBeforePause = _particleSystem.isEmitting;
                _particleSystem.Pause();
            }
            else
            {
                if (_wasEmittingBeforePause)
                {
                    _particleSystem.Play();
                }
            }
        }
    }
}
