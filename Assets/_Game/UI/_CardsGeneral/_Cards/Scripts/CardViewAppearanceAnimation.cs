using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardViewAppearanceAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _cardBgTransform;
        [SerializeField] private float _newCardScale;
        [SerializeField] private float _scaleAnimationDuration;

        [SerializeField] private Image _rippleImage;
        [SerializeField] private RectTransform _rippleTransform;
        [SerializeField] private float _rippleAnimationDuration;
        [SerializeField] private float _rippleScale = 1.3f;

        [SerializeField] private ImageFlashEffect _appearanceImageFlash;
        [SerializeField] private ImageAlphaFlashEffect _rippleImageAlphaFlash;
        
        public void SetColor(Color color)
        {
            _appearanceImageFlash.SetColor(color);
            _rippleImageAlphaFlash.SetColor(color);
        }

        public void PlaySimple() => _appearanceImageFlash.TriggerFlash();

        public void PlayRipple(Action callBack)
        {
            _appearanceImageFlash.TriggerFlash(() => OnAppearanceFinished(callBack));
        }

        private void OnAppearanceFinished(Action callBack)
        {
            PlayRippleAnimation();
            callBack?.Invoke();
        }

        private void PlayRippleAnimation()
        {
            _cardBgTransform.DOScale(_newCardScale, _scaleAnimationDuration / 2)
                .OnComplete(() =>
                    _cardBgTransform.DOScale(1, _scaleAnimationDuration / 2));
            
            _rippleImage.enabled = true;
            _rippleImageAlphaFlash.TriggerFlash();
            _rippleTransform.DOScale(_rippleScale, _rippleAnimationDuration);
            
            DOVirtual.DelayedCall(_rippleAnimationDuration, () => _rippleImage.enabled = false);
        }
    }
}