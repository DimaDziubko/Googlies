using _Game.Utils;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Gameplay._Coins.Scripts
{
    public class LootFlyingReward : FlyingReward
    {
        //Animation data
        [SerializeField, FloatRangeSlider(1.2f, 1.7f)]
        private FloatRange _jumpDistance = new FloatRange(1.2f, 1.7f);

        [SerializeField, FloatRangeSlider(0.3f, 0.6f)]
        private FloatRange _jumpPower = new FloatRange(0.3f, 0.6f);


        [SerializeField] private float[] _directionAngles;

        [SerializeField, FloatRangeSlider(-10f, 10f)] 
        private FloatRange _directionDeviation = new FloatRange(-10f, 10f);


        [SerializeField] private int _numJumps = 2;
        [SerializeField] private float _jumpDuration = 1f;

        [SerializeField] private float _scaleDuration = 1f;

        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private float _moveDelay = 1f;

        private Vector3 _targetPoint;
        
        private float _amount;
        
        private Tween _scaleTween;
        private Tween _jumpTween;
        private Tween _moveTween;
        private Tween _delayTween;

        public void Init(Vector3 targetPoint)
        {
            _targetPoint = targetPoint;
            _transform.localScale = Vector3.zero;
        }

        [Button]
        public void Jump()
        {
            CleanupTweens();

            float angle = _directionAngles[Random.Range(0, _directionAngles.Length)];
            float deviatedAngle = angle + _directionDeviation.RandomValueInRange;
            float rad = deviatedAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized * _jumpDistance.RandomValueInRange;
            Vector3 finalJumpPosition = Position + offset;

            _scaleTween = _transform.DOScale(Vector3.one, _scaleDuration);
            _jumpTween = _transform.DOJump(finalJumpPosition, _jumpPower.RandomValueInRange, _numJumps, _jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _delayTween = DOVirtual.DelayedCall(_moveDelay, MoveToTarget);
                });
        }

        private void MoveToTarget()
        {
            _moveTween = _transform.DOMove(_targetPoint, _moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(Recycle);
        }

        private void CleanupTweens()
        {
            _scaleTween?.Kill();
            _jumpTween?.Kill();
            _moveTween?.Kill();
            _delayTween?.Kill();
        }

        public override void Recycle()
        {
            CleanupTweens();
            base.Recycle();
        }
    }
}