using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [ExecuteAlways, ExecuteInEditMode]
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public class ButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform _buttonTransform;
        private Button _button;

        [SerializeField] private float _pressWarp = 0.9f;
    
        private float _normalHeight;
        private float _pressedHeight;

        private void Awake()
        {
            _buttonTransform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
        
            _normalHeight = _buttonTransform.rect.height;
            _pressedHeight = _normalHeight * _pressWarp;
        }
    
        [Button]
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!_button.interactable) return;
            _buttonTransform.sizeDelta = new Vector2(_buttonTransform.sizeDelta.x, _pressedHeight);
        }

        [Button]
        public void OnPointerUp(PointerEventData eventData) => 
            _buttonTransform.sizeDelta = new Vector2(_buttonTransform.sizeDelta.x, _normalHeight);
    }
}