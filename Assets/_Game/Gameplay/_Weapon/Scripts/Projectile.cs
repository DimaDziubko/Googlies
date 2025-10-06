using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon._Projectile;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Common;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts 
{
    public class Projectile : GameBehaviour
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Pivot _pivot;

        [SerializeField] private Rotator _rotator;
        
        [ShowInInspector, ReadOnly]
        private ProjectileMove _move;
        private ProjectileImpact _impact;

        private ISoundService _soundService;

        [ShowInInspector] 
        protected ITarget _target;

        private IVFXProxy _vfxProxy;

        public IProjectileFactory OriginFactory { get; set; }

        private bool _isInitialized;
        
        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        [ShowInInspector, ReadOnly]
        public int WeaponId { get; private set; }

        public Faction Faction { get; private set; }
        
        [ShowInInspector, ReadOnly]
        protected bool _isDead;

        private IWeaponData _weaponData;
        private bool _isPaused;

        public void Construct(
            ISoundService soundService,
            Faction faction,
            IWeaponData weaponData,
            ProjectileMove move,
            ProjectileImpact impact)
        {
            _impact = impact;
            _move = move;
            
            _soundService = soundService;

            Faction = faction;
            _weaponData = weaponData;

            WeaponId =  weaponData.WeaponId;

            gameObject.layer = weaponData.Layer;
            
            _isDead = false;
        }

        public override bool GameUpdate(float deltaTime)
        {
            if (_isDead)
            {
                Recycle();
                return false;
            }
            
            if (_isPaused)
                return true;

            if (_isInitialized) _move.Move(deltaTime);

            if (_rotator) _rotator.Rotate(deltaTime);

            return true;
        }
        
        public void PrepareIntro(
            IVFXProxy vfxProxy,
            Vector3 launchPosition,
            ITarget target,
            Quaternion rotation,
            float speedFactor,
            Vector3 projectileScale)
        {
            _vfxProxy = vfxProxy;
            Position = launchPosition;
            Rotation = rotation;
            _target = target;
            
            _pivot.Scale = projectileScale;
            
             float localPivotRotationX = 0;
            
             if (_target.Transform.position.x - launchPosition.x < 0)
             {
                 localPivotRotationX = 180;
             }
            
            _pivot.Rotation = Quaternion.Euler(localPivotRotationX, 0, 0); 
            
            SetSpeedFactor(speedFactor);
            
            _move.Initialize(_transform, _target, launchPosition, _weaponData.ProjectileSpeed, _weaponData.TrajectoryWarpFactor, OnTargetReached);
            _impact.Initialize(_transform, _target, _weaponData, OnImpact);
            _isInitialized = true;
        }

        public override void Recycle()
        {
            _target = null;
            _vfxProxy = null;
            OriginFactory.Reclaim(this);
        }

        private void OnTargetReached() => _impact.Hit();

        private void OnImpact()
        {
            SpawnVfx();
            PlaySound();
            _isDead = true;
        }

        private void SpawnVfx()
        {
            if (_weaponData.ProjectileExplosionKey != Constants.ConfigKeys.MISSING_KEY)
            {
                _vfxProxy.SpawnProjectileExplosion(_weaponData.ProjectileExplosionKey, Position).Forget();
            }
        }

        private void PlaySound()
        {
            if (_soundService != null && _weaponData.ProjectileSfx != null)
            {
                _soundService.CreateSound()
                    .WithSoundData(_weaponData.ProjectileSfx)
                    .WithRandomPitch()
                    .WithPosition(Vector3.zero)
                    .Play();
            }
        }
        
        public override void SetSpeedFactor(float speedFactor) => 
            _move.SetSpeedFactor(speedFactor);

        public override void SetPaused(in bool isPaused) => _isPaused = isPaused;
    }
}