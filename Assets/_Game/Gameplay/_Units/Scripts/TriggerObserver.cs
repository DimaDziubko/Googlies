using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObserver : MonoBehaviour
    {
        [SerializeField, Required] private CircleCollider2D _collider;
        public event Action<Collider2D> TriggerEnter;
        public event Action<Collider2D> TriggerExit;

        public void Initialize(in int layer)
        {
            gameObject.layer = layer;
        }

        public void SetSize(float radius)
        {
            if (_collider != null)
            {
                _collider.radius = radius;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        private void OnTriggerEnter2D(Collider2D other) => 
            TriggerEnter?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) => 
            TriggerExit?.Invoke(other);
    }
}
