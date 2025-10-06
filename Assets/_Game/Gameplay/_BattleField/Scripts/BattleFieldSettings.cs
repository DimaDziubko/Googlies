using _Game.Core.Services._Camera;
using _Game.Utils;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    [CreateAssetMenu(fileName = "BattleFieldSettings", menuName = "BattleField/BattleFieldSettings")]
    public class BattleFieldSettings : ScriptableObject
    {
        private const float BASE_COLLIDER_SIZE_X = 0.34f;
        private const float UNIT_SPAWN_Y_CORRECTION = 0.025f;

        [Header("Spawn Settings")]
        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _playerXSpawnRange = new(-5, 5f);

        [SerializeField]
        [FloatRangeSlider(0, 5f)] 
        private FloatRange _playerYSpawnRange = new(0, 5f);

        [SerializeField]
        [FloatRangeSlider(0, 5f)] 
        private FloatRange _enemyXSpawnRange = new(0, 5f);

        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _enemyYSpawnRange = new(-5, 5f);
        
        [Space(10)]
        [Header("Destination Settings")]
        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _playerXDestinationRange = new(0, 1f);

        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _playerYDestinationRange = new(-2, 2);

        [SerializeField]
        [FloatRangeSlider(0, 5f)] 
        private FloatRange _enemyXDestinationRange = new(0, 5f);

        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _enemyYDestinationRange = new(-5, 5f);
        
        [SerializeField]
        [FloatRangeSlider(-5, 5f)] 
        private FloatRange _battleFieldYRange = new(-5, 5f);
        
        private readonly float _baseXOffset = 0.5f;

        public GameMode GameMode;
        public Vector3 PlayerBasePoint => _playerBasePoint;
        public Vector3 EnemyBasePoint => _enemyBasePoint;
        
        public Vector3 PlayerSpawnPoint => 
            new (-_cameraService.CameraWidth - _playerXSpawnRange.RandomValueInRange,
                -UNIT_SPAWN_Y_CORRECTION + _playerYSpawnRange.RandomValueInRange,
                0);
        
        public Vector3 EnemySpawnPoint => 
            new (_cameraService.CameraWidth + _enemyXSpawnRange.RandomValueInRange,
                -UNIT_SPAWN_Y_CORRECTION + _enemyYSpawnRange.RandomValueInRange,
                0);
        
        
        public Vector3 PlayerDestinationPoint => 
            new (_playerXDestinationRange.RandomValueInRange,
                _playerYDestinationRange.RandomValueInRange,
                0);
        
        public Vector3 EnemyDestinationPoint => 
            new (_enemyXDestinationRange.RandomValueInRange,
                _enemyYDestinationRange.RandomValueInRange,
                0);

        public Vector3 GetRandomTopPoint()
        {
            FloatRange randomXRange = new FloatRange(-_cameraService.CameraWidth, _cameraService.CameraWidth);
            return new Vector3(randomXRange.RandomValueInRange, _cameraService.CameraHeight, 0);
        }
        
        public Vector3 GetRandomPointOnFieldWithBaseOffset()
        {
            FloatRange randomXRange = new FloatRange(-_cameraService.CameraWidth + _baseXOffset, _cameraService.CameraWidth - _baseXOffset);
            return new Vector3(randomXRange.RandomValueInRange, _battleFieldYRange.RandomValueInRange, 0);
        }
        
        public Vector3 StrengthPowerUpPosition => PlayerBasePoint + _baseFrontPowerUpOffset;
        public float PlayerBaseBound => _playerBasePoint.x + BASE_COLLIDER_SIZE_X;
        public float EnemyBaseBound => _enemyBasePoint.x - BASE_COLLIDER_SIZE_X;

        private IWorldCameraService _cameraService;

        private Vector3 _enemyBasePoint;
        private Vector3 _playerBasePoint;
        private Vector3 _baseFrontPowerUpOffset = new(1, 0, 0);

        public void Initialize(IWorldCameraService cameraService)
        {
            _cameraService = cameraService;
            CalculateBasePoints();
        }
        
        // private void OnValidate()
        // {
        //     _playerYSpawnRange.Clamp(-_cameraService.CameraHeight, _cameraService.CameraHeight);
        //     _enemyYSpawnRange.Clamp(-_cameraService.CameraHeight, _cameraService.CameraHeight);
        //     _playerYDestinationRange.Clamp(-_cameraService.CameraHeight, _cameraService.CameraHeight);
        //     _enemyYDestinationRange.Clamp(-_cameraService.CameraHeight, _cameraService.CameraHeight);
        //     _playerXDestinationRange.Clamp(-_cameraService.CameraWidth, _cameraService.CameraWidth);
        //     _enemyXDestinationRange.Clamp(-_cameraService.CameraWidth, _cameraService.CameraWidth);
        // }
        
        private void CalculateBasePoints()
        {
            _enemyBasePoint = new Vector3(_cameraService.CameraWidth, 0, 0);
            _playerBasePoint = new Vector3(-_cameraService.CameraWidth, 0, 0);
        }
    }
}