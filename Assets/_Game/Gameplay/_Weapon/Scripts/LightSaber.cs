using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class LightSaber : WeaponStrategy
    {
        public LightSaber(ISoundService soundService) : base(soundService)
        {
        }

        public override void Initialize(
            IWeaponData weaponData, 
            IVFXProxy vfxProxy, 
            IShootProxy shootProxy,
            Transform generator,
            Transform aim,
            Transform weaponBone,
            WeaponColorizer colorizer, 
            Faction faction,
            IMyLogger logger)
        {
            base.Initialize(weaponData, vfxProxy, shootProxy, generator, aim, weaponBone, colorizer, faction, logger);
            _logger.Log("Initialized LightSaber", DebugStatus.Info);
            _colorizer.SetWeaponColor(weaponData.Color);
        }

        public override void EnableWeapon()
        {
            PlayEnableSound();
        }

        public override void Attack()
        {
            if (_target.IsValid())
                _target.TakeDamage(_weaponData.Damage);
            PlayAttackSound();
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

        private void PlayEnableSound()
        {
            if (_soundService != null && _weaponData.WeaponEnableSfx != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_weaponData.WeaponEnableSfx)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }
        
        private void PlayAttackSound()
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