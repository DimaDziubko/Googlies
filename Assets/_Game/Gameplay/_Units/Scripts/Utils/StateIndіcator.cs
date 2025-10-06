using UnityEngine;

namespace _Game.Gameplay._Units.Scripts.Utils
{
    public class StateIndіcator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _indicator;

        public void SetColor(Color color)
        {
            if(_indicator.color == color) return; 
            _indicator.color = color;
        }
    }
}
