using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _point;
    
        public Vector3 Position
        {
            get => _transform.position;
            set
            {
                _transform.position = _point.position = value;
            } 
        }

        public Vector3 Direction;
        public float Speed;
    
        // Update is called once per frame
        void Update()
        {
            Vector3 newPos = Position;
            newPos += Speed * Direction * Time.deltaTime;
            Position = newPos;
        }
    }
}
