using _Game.Core.Services.Audio;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class SimpleMelee : WeaponStrategy
    {
        public SimpleMelee(ISoundService soundService) : base(soundService)
        {
        }

        public override void Attack()
        {
            if (_target.IsValid())
                _target.TakeDamage(_weaponData.Damage);
            PlaySound();
        }
        
        public override void SpecialAttack()
        {
            if (_target.IsValid())
                _target.TakeDamage(_weaponData.Damage);
            PlaySpecialAttackSound();
        } 

        private void PlaySpecialAttackSound()
        {
            if (_soundService != null && _weaponData.SpecialAttackSfx != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_weaponData.SpecialAttackSfx)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }
        
        private void PlaySound()
        {
            if (_soundService != null && _weaponData.AttackSfx != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_weaponData.AttackSfx)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }
    }
}