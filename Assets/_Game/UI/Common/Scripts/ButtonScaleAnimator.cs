using TMPro;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    public class ButtonScaleAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private RectTransform _contentTransform;

        [SerializeField] private float _targetScaleX = 1;
        [SerializeField] private float _targetScaleY = 1;
        
        [SerializeField] private TMP_Text _label;
        [SerializeField] private int _normalFontSize;
        [SerializeField] private int _highligtedFontSize;

        private Vector2 _normalButtonSize;
        private Vector2 _highlightedButtonSize;

        private Vector2 _normalContentSize;
        private Vector2 _highlightedContentSize;

        private bool _isInitialized = false;

        public void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_buttonTransform != null)
            {
                _normalButtonSize = _buttonTransform.sizeDelta;
                _highlightedButtonSize =
                    new Vector2(_normalButtonSize.x * _targetScaleX, _normalButtonSize.y * _targetScaleY);
            }

            if (_contentTransform != null)
            {
                _normalContentSize = _contentTransform.sizeDelta;
                _highlightedContentSize = new Vector2(_normalContentSize.x * _targetScaleX,
                    _normalContentSize.y *  _targetScaleY);
            }

            _isInitialized = true;
        }

        public void Highlight()
        {
            if (!_isInitialized) Initialize();

            if (_buttonTransform != null)
                _buttonTransform.sizeDelta = _highlightedButtonSize;

            if (_contentTransform != null)
                _contentTransform.sizeDelta = _highlightedContentSize;
            
            if (_label != null)
            {
                _label.fontSize = _highligtedFontSize;
            }
        }

        public void UnHighlight()
        {
            if (_buttonTransform != null)
                _buttonTransform.sizeDelta = _normalButtonSize;

            if (_contentTransform != null)
                _contentTransform.sizeDelta = _normalContentSize;
            
            if (_label != null)
            {
                _label.fontSize = _normalFontSize;
            }
        }
    }
}