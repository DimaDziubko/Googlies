using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    [ExecuteAlways]
    public class TiggerTest : MonoBehaviour
    {
        private Collider _collider;
    
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Enter");
            _collider = other;
        }
    
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Exit");
            _collider = null;
        }

        [Button]
        public void CheckCollider()
        {
            Debug.Log($"Has collider {_collider != null}");
        }
    }
}
