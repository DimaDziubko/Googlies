using System;
using System.Collections;
using _Game.Core.Pause.Scripts;
using _Game.Utils;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Movement
{
    public interface IMovable
    {
        Vector3 Position { get; }
        public bool IsMoving { get; }
        void Move(Vector3 destination);
    }
    
    [RequireComponent(typeof(AIPath), typeof(Rigidbody2D))]
    public class AUnitMove : MonoBehaviour, IMovable, IPauseHandler
    {
        [SerializeField, Required] private Seeker _seeker;
        [SerializeField, Required] private AIPath _aiPath;
        [SerializeField, Required] private Transform _unitTransform;
        [SerializeField] private float _smoothTime = 0.1f;

        public Vector3 Position => _unitTransform.position;

        public float DefaultSpeed { get; private set; }

        private Vector3 _lastFixedPosition;
        private Vector3 _lastFramePosition;

        private float _updateInterval = 2f;
        private float _lastUpdateTime = 0;
        private float _tempMoveSpeedFactor;
        private float _smoothedSpeed;

        private Coroutine _restoreCoroutine;

        //[ShowInInspector, ReadOnly]
        public bool IsActuallyMoving => Vector3.SqrMagnitude(Position - _lastFixedPosition) > float.Epsilon;
        
        //[ShowInInspector, ReadOnly]
        public bool IsMoving => !_aiPath.isStopped && _aiPath.reachedEndOfPath == false;
        
        private Quaternion Rotation
        {
            get => _unitTransform.rotation;
            set => _unitTransform.rotation = value;
        }

        
        public void Construct(float speed)
        {
            _aiPath.maxSpeed = DefaultSpeed = speed;
        }

        public void Move(Vector3 destination)
        {
            if (_aiPath.isStopped)
            {
                _aiPath.isStopped = false;
            }
            
            RotateToTarget(destination);
            
            _aiPath.destination = destination;
            _aiPath.SearchPath();

            if (Time.time - _lastUpdateTime > _updateInterval) {
                _lastFixedPosition = Position;
                _lastUpdateTime = Time.time;
            }
        }
        
        private void RotateToTarget(Vector3 destination)
        {
            if (destination.x < Position.x - Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (destination.x > Position.x + Constants.ComparisonThreshold.UNIT_ROTATION_EPSILON)
            {
                Rotation = Quaternion.Euler(0, 0, 0);
            }
        }


        public void Stop()
        {
            _aiPath.destination = Position;
            //_rigidbody2D.linearVelocity = Vector3.zero;
        }

        public void SetSpeed(float speed)
        {
            _aiPath.maxSpeed = speed;
        }
        
        public Vector3 Destination { get; set; }
        public Vector3 DeviationPoint { get; set; }
        public float SpeedFactor => _aiPath.maxSpeed;


        [Button]
        private void ManualInit()
        {
            _aiPath = GetComponent<AIPath>();
        }

        public void SetPaused(bool isPaused)
        {
            if (isPaused)
            {
                _tempMoveSpeedFactor = SpeedFactor;
            }
            
            SetSpeed(isPaused ? 0 : _tempMoveSpeedFactor);
        }

        public void SetSpeedFactor(float speedFactor)
        {
            SetSpeed(DefaultSpeed * speedFactor);
        }

        public void Cleanup()
        {
            if (_restoreCoroutine != null)
            {
                StopCoroutine(_restoreCoroutine);
            }
            _aiPath.enabled = true;
            _aiPath.SearchPath();
        }

        public void Disable()
        {
            _seeker.enabled = false;
            _aiPath.enabled = false;
        }

        public void Enable()
        {
            _seeker.enabled = true;
            _aiPath.enabled = true;
        }

        public float GetCurrentSpeed()
        {
            Vector3 delta = Position - _lastFramePosition;
            float targetSpeed = delta.magnitude / Time.deltaTime;
            _smoothedSpeed = Mathf.Lerp(_smoothedSpeed, targetSpeed, Time.deltaTime / _smoothTime);
            _lastFramePosition = Position;
            return _smoothedSpeed;
        }
        
        // private void OnDrawGizmos()
        // {
        //     // If the destination vector is set
        //     if (Destination != Vector3.zero)
        //     {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawSphere(Destination, 0.1f); 
        //     }
        //     
        //     if (DeviationPoint != Vector3.zero && DeviationPoint != Destination)
        //     {
        //         Gizmos.color = Color.green;
        //         Gizmos.DrawSphere(DeviationPoint, 0.1f); 
        //     }
        // }
    }
}
