using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class ScaleAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Vector2 _startScale = new Vector2(0f, 0f);
        [SerializeField] private Vector2 _targetScale = new Vector2(1.2f, 1.2f);
        [SerializeField] private Vector2 _normalScale = new Vector2(1f, 1f);

        [SerializeField] private float _duration = 1f;

        private Tweener _scaleTween; 
        
        public void Init()
        {
            _transform.localScale = _startScale;
        }

        public async UniTask PlayAsync()
        {
            _scaleTween?.Kill();

            _transform.localScale = _startScale;
            
            _scaleTween = _transform.DOScale(_targetScale, _duration / 2)
                .OnComplete(() => 
                    _scaleTween = _transform.DOScale(_normalScale, _duration / 2)
                );

            await _scaleTween.AsyncWaitForCompletion();
        }
        
        public void Play()
        {
            _scaleTween?.Kill();

            _transform.localScale = _startScale;
            
            _scaleTween = _transform.DOScale(_targetScale, _duration / 2)
                .OnComplete(() => 
                    _scaleTween = _transform.DOScale(_normalScale, _duration / 2)
                );
        }
        
        public void Cleanup()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
        }
    }
}