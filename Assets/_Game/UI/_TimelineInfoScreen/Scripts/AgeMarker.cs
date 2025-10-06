using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class AgeMarker : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _emptyMarker;
        [SerializeField] private Image _filledMarker;
        
        //Animation data
        [SerializeField] private float _animationScale = 1.2f;
        [SerializeField] private float _animationFade = 0.5f;
        [SerializeField] private int _loopCount = 1;
        
        private Tween _scaleAnimation;
        private Tween _iconAnimation;
        
        public void SetFilled(bool isFilled)
        {
            _emptyMarker.enabled = !isFilled;
            _filledMarker.enabled = isFilled;
        }

        public void PlayRippleAnimation(in float rippleAnimationDuration)
        {
            Cleanup();

            _emptyMarker.enabled = false;
            _filledMarker.enabled = true;
            
            _scaleAnimation = _rectTransform.DOScale(_animationScale, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
            _iconAnimation = _filledMarker.DOFade(_animationFade, rippleAnimationDuration / _loopCount)
                .SetEase(Ease.InOutQuad)
                .SetLoops(_loopCount, LoopType.Yoyo);
        }

        public void Cleanup()
        {
            _scaleAnimation?.Kill();
            _iconAnimation?.Kill();

            _scaleAnimation = null;
            _iconAnimation = null;
        }
    }
}