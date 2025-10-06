using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Gameplay._Units.Scripts.Movement
{
    public class UnitMove : MonoBehaviour, IMovable
    {
        [SerializeField] private NavMeshAgent _agent;
        
        private Transform _unitTransform;
        public Vector3 Position => _unitTransform.position;
        public bool IsMoving => !_agent.isStopped && _agent.velocity.sqrMagnitude > 0.01f;
        private Quaternion Rotation
        {
            get => _unitTransform.rotation;
            set => _unitTransform.rotation = value;
        }

        private float _speed;
        
        public void Construct(Transform unitTransform, float speed)
        {
            _unitTransform = unitTransform;
            _agent.speed = speed;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.acceleration = 8;
        }
        
        public void Move(Vector3 destination)
        {
            if (_agent.isStopped)
            {
                _agent.isStopped = false;
            }

            RotateToTarget(destination);
            
            _agent.SetDestination(destination);
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
            _agent.isStopped = true;
        }

        public Vector3 Destination { get; set; }
        public Vector3 DeviationPoint { get; set; }
        
        public float SpeedFactor => _agent.speed;

        //TODO Delete 
        private void OnDrawGizmos()
        {
            // If the destination vector is set
            if (Destination != Vector3.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(Destination, 0.1f); 
            }
            
            if (DeviationPoint != Vector3.zero && DeviationPoint != Destination)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(DeviationPoint, 0.1f); 
            }
        }
        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        //Helper
        [Button]
        private void ManualInit()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }
}