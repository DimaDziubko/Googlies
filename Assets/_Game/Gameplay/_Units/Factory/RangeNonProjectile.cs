using _Game.Core.Services.Audio;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._Units.Factory
{
    public class RangeNonProjectile : WeaponStrategy
    {
        public RangeNonProjectile(ISoundService soundService) : base(soundService)
        {
        }

        public override void Attack()
        {
            if (!TryToShoot()) return;
            PlaySound();
        }

        public override void SpecialAttack()
        {
            if (!TryToShoot()) return;
            PlaySpecialAttackSound();
        }

        public override void LateGameUpdate(float deltaTime) =>
            CalculateAimPosition(deltaTime);

        private void CalculateAimPosition(float deltaTime)
        {
            if (!_weaponData.IsAiming || !_target.IsValid())
                return;

            Vector2 start = _weaponBone.position;
            Vector2 end = _target.Transform.position;

            var settings = _weaponData.AimingSettings;
            float minAngle = settings.AngleRange.Min;
            float maxAngle = settings.AngleRange.Max;

            Vector2 toTarget = (end - start).normalized;
            if (toTarget.sqrMagnitude < 0.001f)
                return;

            bool isRight = toTarget.x >= 0;
            
            Vector2 baseDir = Vector2.right;

            float rawAngle = Vector2.SignedAngle(baseDir, toTarget);
            float clampedAngle;

            if (isRight)
            {
                clampedAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);
            }
            else
            {
                float leftAngle = rawAngle > 0 ? rawAngle - 180f : rawAngle + 180f;
                float clampedLeft = Mathf.Clamp(leftAngle, -maxAngle, -minAngle);
                clampedAngle = clampedLeft > 0 ? clampedLeft + 180f : clampedLeft - 180f;
            }

            float angleRad = clampedAngle * Mathf.Deg2Rad;
            Vector2 clampedDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            Vector3 targetPosition = (Vector3)start + (Vector3)(clampedDir.normalized * (end - start).magnitude);

            _aim.position = Vector3.Lerp(_aim.position, targetPosition, deltaTime / settings.AimSmoothTime);
        }

        private bool TryToShoot()
        {
            if (!_target.IsValid()) return false;

            if (_weaponData.MuzzleKey != Constants.ConfigKeys.MISSING_KEY)
            {
                var muzzleData = new MuzzleData
                {
                    WeaponId = _weaponData.WeaponId,
                    Direction = _generator.forward,
                    Position = _generator.position,
                    MuzzleKey = _weaponData.MuzzleKey
                };

                _vfxProxy.SpawnMuzzleFlash(muzzleData).Forget();
            }

            if (_target.IsValid())
                _target.TakeDamage(_weaponData.Damage);

            return true;
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