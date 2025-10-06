using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._PickUp
{
    public class PickUp : PickUpBase
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private AudioClip _pickUpSound;
        [SerializeField, Required] private  Collider2D _collider2D;
        
        private StrengthPowerUp _strengthPowerUp;
        private IAudioService _audioService;
        
        private bool _isPickedUp;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
            _collider2D.enabled = true;
            _isPickedUp = false;
        }
        
        public void SetPowerUp(StrengthPowerUp strengthPowerUp)
        {
            _strengthPowerUp = strengthPowerUp;
        }
        
        public Vector3 Position
        {
            get => _transform.position; 
            set => _transform.position = value;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isPickedUp) return;

            IVisitable visitable = other.GetComponent<IVisitable>();
            if (visitable != null)
            {
                _isPickedUp = true;
                _collider2D.enabled = false;
                visitable.Accept(_strengthPowerUp);
                PlaySfx();
                Recycle();
            }
        }
        
        private void PlaySfx()
        {
            if (_audioService != null &&  _pickUpSound != null)
            {
                _audioService.PlayOneShot(_pickUpSound);
            }
        }
    }
}