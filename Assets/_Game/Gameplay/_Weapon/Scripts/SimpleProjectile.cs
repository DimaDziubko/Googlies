using _Game.Core.Services.Audio;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class SimpleProjectile : WeaponStrategy
    {
        private readonly TrajectoryFactory _trajectoryFactory;

        public SimpleProjectile(ISoundService soundService, TrajectoryFactory trajectoryFactory) : base(soundService)
        {
            _trajectoryFactory = trajectoryFactory;
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
            
            float distance = Vector2.Distance(start, end);
            
            float adjustedWarp = _weaponData.TrajectoryWarpFactor * distance;

            var trajectory = _trajectoryFactory.Get(_weaponData.TrajectoryType);
            var settings = _weaponData.AimingSettings;

            float minAngle = settings.AngleRange.Min;
            float maxAngle = settings.AngleRange.Max;

            var rawDir = trajectory.GetInitialDirection(start, end, adjustedWarp);
            if (rawDir.sqrMagnitude < 0.001f)
                return;
            
            bool isRight = rawDir.x >= 0;
            Vector2 baseDir = Vector2.right;
            float rawAngle = Vector2.SignedAngle(baseDir, rawDir);

            float clampedAngle;

            if (isRight)
            {
                clampedAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);
            }
            else
            {
                float leftRelative = rawAngle > 0 ? 180 - rawAngle : -180 - rawAngle;
                float clampedLeft = Mathf.Clamp(leftRelative, minAngle, maxAngle);
                clampedAngle = rawAngle > 0 ? 180 - clampedLeft : -180 - clampedLeft;
            }

            float angleRad = clampedAngle * Mathf.Deg2Rad;
            Vector2 clampedDir = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            Vector3 targetPosition = (Vector3)start + (Vector3)(clampedDir.normalized * rawDir.magnitude);

            _aim.position = Vector3.Lerp(_aim.position, targetPosition, deltaTime / settings.AimSmoothTime);
        }

        private bool TryToShoot()
        {
            if (!_target.IsValid()) return false;

            var data = new ShootData
            {
                Faction = _faction,
                Target = _target,
                WeaponData = _weaponData,
                LaunchPosition = _generator.position,
                LaunchRotation = _generator.rotation,
                ProjectileScale = _weaponScale,
            };

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

            _shootProxy.Shoot(data).Forget();
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