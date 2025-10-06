using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class ToggleButtonStateAnimator : MonoBehaviour
    {
        [SerializeField] private float _targetScaleX = 1;
        [SerializeField] private float _targetScaleY = 1;
        [SerializeField] private float _highlightedIconScale = 1.3f;

        private RectTransform _buttonTransform;
        private Vector2 _normalButtonSize;
        private Vector2 _highlightedButtonSize;

        private RectTransform _iconTransform;
        private Vector2 _normalIconSize;
        private Vector2 _highlightedIconSize;

        public void Initialize(
            RectTransform buttonTransform,
            RectTransform iconTransform)
        {
            if (buttonTransform != null)
            {
                _buttonTransform = buttonTransform;
                _normalButtonSize = _buttonTransform.sizeDelta;
                _highlightedButtonSize = new Vector2(_normalButtonSize.x * _targetScaleX, _normalButtonSize.y * _targetScaleY);
            }
            
            if (iconTransform != null)
            {
                _iconTransform = iconTransform;
                _normalIconSize = _iconTransform.sizeDelta;
                _highlightedIconSize = new Vector2(_normalIconSize.x *_highlightedIconScale, _normalIconSize.y * _highlightedIconScale);
            }
        }

        public void Highlight()
        {
            if (_buttonTransform != null)
                _buttonTransform.sizeDelta = _highlightedButtonSize;
            if(_iconTransform != null)
                _iconTransform.sizeDelta = _highlightedIconSize;
        }
        public void UnHighlight()
        {
            if (_buttonTransform != null)
                _buttonTransform.sizeDelta = _normalButtonSize;
            if(_iconTransform != null)
                _iconTransform.sizeDelta = _normalIconSize;
        }
    }
}