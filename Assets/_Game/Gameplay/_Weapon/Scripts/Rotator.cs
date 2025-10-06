using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
        [SerializeField] private bool _clockwise = true;

        public void SetSpeed(float speed)
        {
            _rotationSpeed = speed;
        }

        public void SetClockwise(bool isClockwise)
        {
            _clockwise = isClockwise;
        }
        
        public void Rotate(float deltaTime)
        {
            float direction = _clockwise ? 1f : -1f;
            _transform.Rotate(_rotationAxis * _rotationSpeed * direction * deltaTime, Space.Self);
        }
    }
}