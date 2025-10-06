using _Game.Utils;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class LootFlyingRewardTest : MonoBehaviour
    {
        [IntRangeSlider(3, 10)]
        public IntRange TestRange = new IntRange(3, 10);
    
        public Vector3 TargetPosition; 
        public Vector3 Direction; 
        public float JumpDistance = 3f; 
        public float JumpPower = 1f; 
        public int NumJumps = 2; 
        public float MoveDuration = 0.5f;

        [Button]
        void Jump()
        {
            transform.localScale = Vector3.zero;
        
            Vector3 jumpDirection = Direction.normalized * JumpDistance;
            Vector3 finalJumpPosition = transform.position + jumpDirection;
        
            transform.DOScale(Vector3.one, 0.5f);
        
            transform.DOJump(finalJumpPosition, JumpPower, NumJumps, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, MoveToTarget);
            });
        }

        void MoveToTarget()
        {
            transform.DOMove(TargetPosition, MoveDuration).SetEase(Ease.Linear);
        }
    }
}
