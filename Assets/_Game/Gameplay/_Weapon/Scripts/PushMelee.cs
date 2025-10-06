using _Game.Core.Services.Audio;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class PushMelee : WeaponStrategy
    {
        public PushMelee(ISoundService soundService) : base(soundService)
        {
        }

        public override void Attack()
        {
            if (_target.IsValid())
            {
                _target.TakeDamage(_weaponData.Damage);

                Vector3 pushDirection = (_target.GetPosition() - GetWeaponPosition()).normalized;
                _target.Push(pushDirection, _weaponData.ImpulseStrength);
            }
                
            PlaySound();
        }
        
        public override void SpecialAttack()
        {
            if (_target.IsValid())
            {
                _target.TakeDamage(_weaponData.Damage);
                
                Vector3 pushDirection = (_target.GetPosition() - GetWeaponPosition()).normalized;
                _target.Push(pushDirection, _weaponData.ImpulseStrength);
            }
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