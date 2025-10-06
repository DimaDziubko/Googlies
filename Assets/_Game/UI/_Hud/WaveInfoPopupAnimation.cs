using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud
{
    public class WaveInfoPopupAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Image _fadableImage;
        [SerializeField] private TextMeshProUGUI _fadableText;

        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _mainPosition;

        [SerializeField] private float _initialDelay = 5f;
        [SerializeField] private float _moveToMainTimeSec = 2;
        [SerializeField] private float _mainPositionDelay = 3;
        [SerializeField] private float _fadeTime = 2;

        private float _normalAlpha = 1f;
        private float _zeroAlpha = 0f;

        private Sequence _currentSequence;

        public void PlayAnimation()
        {
            gameObject.SetActive(true); 
        
            var imageColor = _fadableImage.color;
            imageColor.a = _zeroAlpha;
            _fadableImage.color = imageColor;

            var textColor = _fadableText.color;
            textColor.a = _zeroAlpha;
            _fadableText.color = textColor;

            _transform.localPosition = _startPosition;
        
            _currentSequence = DOTween.Sequence();
        
            _currentSequence.PrependInterval(_initialDelay);
        
            _currentSequence.Append(_transform.DOLocalMove(_mainPosition, _moveToMainTimeSec).SetEase(Ease.OutQuad));
        
            _currentSequence.Join(_fadableImage.DOFade(_normalAlpha, _moveToMainTimeSec));
            _currentSequence.Join(_fadableText.DOFade(_normalAlpha, _moveToMainTimeSec));
        
            _currentSequence.AppendInterval(_mainPositionDelay);

            _currentSequence.Append(_fadableImage.DOFade(_zeroAlpha, _fadeTime));
            _currentSequence.Join(_fadableText.DOFade(_zeroAlpha, _fadeTime));
        
            _currentSequence.OnComplete(() => gameObject.SetActive(false));

            _currentSequence.Play();
        }
        
        public void StopAnimation()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
                _currentSequence = null;
            }
            
            _transform.DOKill();
            _fadableImage.DOKill();
            _fadableText.DOKill();

            gameObject.SetActive(false);
        }
    }
}
