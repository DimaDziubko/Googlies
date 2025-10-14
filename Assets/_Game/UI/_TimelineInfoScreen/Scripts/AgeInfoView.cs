using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class AgeInfoView : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private RectTransform _iconTransform;

        [SerializeField] private Image _ageIconHolder;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _dateRangeLabel;
        [SerializeField] private TMP_Text _descriptionLabel;

        [SerializeField] private TMP_Text _lockText;

        [SerializeField] private AgeMarker _marker;

        //Animation data
        [SerializeField] private float _animationScale = 1.2f;
        [SerializeField] private float _animationFade = 0.5f;
        [SerializeField] private int _loopCount = 2;

        private Tween _scaleAnimation;
        private Tween _iconAnimation;

        public RectTransform Transform => _transform;

        public void SetName(string name) => _nameLabel.text = name;
        public void SetIcon(Sprite icon) => _ageIconHolder.sprite = icon;
        public void SetDataRange(string dateRange) => _dateRangeLabel.text = dateRange;
        public void SetDescription(string description) => _descriptionLabel.text = description;

        public RectTransform MarkerRect => _marker.GetComponent<RectTransform>();

        public void SetLocked(bool isLocked)
        {
            _ageIconHolder.enabled = !isLocked;
            _nameLabel.enabled = !isLocked;
            _dateRangeLabel.enabled = !isLocked;
            _descriptionLabel.enabled = !isLocked;
            _lockText.enabled = isLocked;
        }

        public void FillMarker(bool isFilled)
        {
            _marker.SetFilled(isFilled);
        }

        public void PlayRippleAnimation(in float duration)
        {
            Cleanup();

            _scaleAnimation = _iconTransform.DOScale(_animationScale, duration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
            _iconAnimation = _ageIconHolder.DOFade(_animationFade, duration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);

            _marker.PlayRippleAnimation(duration);
        }

        public void Cleanup()
        {
            _scaleAnimation?.Kill();
            _iconAnimation?.Kill();

            _scaleAnimation = null;
            _iconAnimation = null;

            _marker.Cleanup();
        }
    }
}