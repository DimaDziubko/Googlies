using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class StatInfoItemAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemTransform;
        [SerializeField] private Sprite _boostArrow;
        [SerializeField] private TMP_Text _statFullValueLabel;
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private float _spriteChangeDuration = 1f;

        [SerializeField] private Vector2 _normalScale = new(1f, 1f);
        [SerializeField] private Vector2 _targetScale = new(1.2f, 1.2f);
        [SerializeField] private float _fadeAndScaleDuration = 1f;

        [SerializeField] private float _animationDelay = 1f;

        private Sprite _defaultSprite;
        private Sequence _currentSequence;

        public void Play(string currentValue, string newValue)
        {
            _defaultSprite = _iconPlaceholder.sprite;
            _statFullValueLabel.text = currentValue;
        
            _itemTransform.localScale = _normalScale; 
            _iconPlaceholder.color = new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 1);
            _statFullValueLabel.color = new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 1);
            
            _currentSequence = DOTween.Sequence();
        
            _currentSequence.AppendInterval(_animationDelay);

            _currentSequence.AppendCallback(() =>
            {
                _iconPlaceholder.sprite = _boostArrow;
            });

            _currentSequence.AppendInterval(_spriteChangeDuration);

            _currentSequence.AppendCallback(() =>
            {
                _itemTransform.localScale = _targetScale;
                _iconPlaceholder.sprite = _defaultSprite;
                _statFullValueLabel.text = newValue;
                _iconPlaceholder.color = new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 0);
                _statFullValueLabel.color = new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 0);
            });

            _currentSequence.Append(_itemTransform.DOScale(_normalScale, _fadeAndScaleDuration));
            _currentSequence.Join(_iconPlaceholder.DOColor(new Color(_iconPlaceholder.color.r, _iconPlaceholder.color.g, _iconPlaceholder.color.b, 1), _fadeAndScaleDuration / 2));
            _currentSequence.Join(_statFullValueLabel.DOColor(new Color(_statFullValueLabel.color.r, _statFullValueLabel.color.g, _statFullValueLabel.color.b, 1), _fadeAndScaleDuration / 2));
        }
        
        public void Cleanup() => _currentSequence?.Kill();
    }
}