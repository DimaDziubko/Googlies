using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class DamageText : VfxEntity
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private RectTransform _transform;
        [SerializeField, Required] private TMP_Text _label;

        [SerializeField] private float _jumpPower = 1f;
        [SerializeField] private float _jumpDuration = 1f;
        [SerializeField] private float _fadeDuration = 1f;

        [SerializeField] private float _leftRotationAngle = 10f;
        [SerializeField] private float _rightRotationAngle = -10f;

        [SerializeField] private Vector2 _leftDirection = new(-0.5f, 0.5f);
        [SerializeField] private Vector2 _rightDirection = new(0.5f, 0.5f);

        private Sequence _currentSequence;
        
        private bool _isCompleted = false;

        public override bool GameUpdate(float deltaTime)
        {
            if (_isCompleted)
            {
                Recycle();
                return false;
            }
            
            return true;
        }

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public void Initialize()
        {
            _canvas.worldCamera = _cameraService.MainCamera;    
        }
        
        public void PlayDamageTextLeft(string value, Vector3 position)
        {
            PlayDamageText(value, position, _leftDirection, _leftRotationAngle);
        }

        public void PlayDamageTextRight(string value, Vector3 position)
        {
            PlayDamageText(value, position, _rightDirection, _rightRotationAngle);
        }

        private void PlayDamageText(string text, Vector3 position, Vector2 direction, float rotationAngle)
        {
            ResetDamageText();

            _label.gameObject.SetActive(true);
            _label.text = text;
            Position = position;

            Vector3 finalJumpPosition = position + new Vector3(direction.x, direction.y, 0);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_transform.DOJump(finalJumpPosition, _jumpPower, 1, _jumpDuration)
                .SetEase(Ease.OutQuad));

            sequence.Join(_label.DOFade(0, _fadeDuration)
                .SetEase(Ease.Linear));

            sequence.Join(_transform.DORotate(new Vector3(0, 0, rotationAngle), _jumpDuration)
                .SetEase(Ease.OutQuad));

            sequence.OnComplete(OnComplete);

            _currentSequence = sequence;
        }

        [Button]
        public void TestPlayLeft()
        {
            PlayDamageTextLeft("100", Vector3.zero);
        }

        [Button]
        public void TestPlayRight()
        {
            PlayDamageTextRight("200", Vector3.zero);
        }

        private void ResetDamageText()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
                _currentSequence = null;
            }
            
            _isCompleted = false;
            
            _label.gameObject.SetActive(false);
            _label.color = new Color(_label.color.r, _label.color.g, _label.color.b, 1);
            
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
            _transform.localScale = Vector3.one;
        }

        private void OnComplete()
        {
            _currentSequence = null;
            
            ResetDamageText();

            _isCompleted = true;
        }

        public override void Recycle()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
                _currentSequence = null;
            }
            
            if (OriginFactory != null)
                OriginFactory.ReclaimDamageText(this);
            
        }
    }
}