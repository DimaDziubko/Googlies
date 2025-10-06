using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [ExecuteAlways, ExecuteInEditMode]
    [RequireComponent(typeof(Button), typeof(RectTransform))]
    public class NewButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button _button;
        
        [SerializeField, Required] private RectTransform _contentTransform;
        
        [SerializeField] private float _pressOffset = 10f;
        [SerializeField] private float _animationDuration = 0.1f;

        private Vector2 _contentOriginalPosition;

        private void Awake()
        {
            _button = GetComponent<Button>();

            if (_contentTransform != null)
            {
                _contentOriginalPosition = _contentTransform.anchoredPosition;
            }
        }

        [Button]
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;
        
            if (_contentTransform != null)
            {
                _contentTransform.anchoredPosition = _contentOriginalPosition - new Vector2(0, _pressOffset);
            }
        }

        [Button]
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_contentTransform != null)
            {
                _contentTransform.anchoredPosition = _contentOriginalPosition;
            }
        }
    }
}