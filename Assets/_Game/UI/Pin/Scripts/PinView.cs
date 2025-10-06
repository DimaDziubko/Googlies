using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI.Pin.Scripts
{
    public class PinView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _rectTransform;
        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);
        public void SetPosition(Vector3 position) => 
            _rectTransform.anchoredPosition = position;
    }
}
