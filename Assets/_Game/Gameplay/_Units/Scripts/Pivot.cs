using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class Pivot : MonoBehaviour
    {
        [SerializeField, Required] Transform _transform;

        public Transform Transform => _transform;

        public Vector3 Position
        {
            get => _transform.localPosition;
            set => _transform.localPosition = value;
        }
        
        public Quaternion Rotation
        {
            get => _transform.localRotation;
            set => _transform.localRotation = value;
        }
        
        public Vector3 Scale
        {
            get => _transform.localScale;
            set => _transform.localScale = value;
        }
    }
}