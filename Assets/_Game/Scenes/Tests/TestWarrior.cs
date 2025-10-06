using _Game.Core._Logger;
using _Game.Gameplay._Units.Scripts._Animation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public enum WarriorAnimationState
    {
        Locomotion,
        Attack,
        Aggro,
        AggroIntro,
    }
    public class TestWarrior : MonoBehaviour
    {
        [SerializeField] private SpineUnitAnimator _animator;

        private IMyLogger _logger = new MyLogger();

        [SerializeField] private float _speed = 0.1f;

        public WarriorAnimationState State;
        
        private void Start()
        {
            State = WarriorAnimationState.Locomotion;
            _animator.Initialize(_logger);
        }

        private void Update()
        {
            // _animator.GameUpdate(Time.deltaTime);
            //
            // var deltaTime = Time.deltaTime;
            //
            // switch (State)
            // {
            //     case WarriorAnimationState.Locomotion:
            //         HandleLocomotion(deltaTime);
            //         break;
            //     case WarriorAnimationState.Attack:
            //         HandleAttack(deltaTime);
            //         break;
            //     case WarriorAnimationState.Aggro:
            //         HandleAggro(deltaTime);
            //         break;
            //     case WarriorAnimationState.AggroIntro:
            //         HandleAggroIntro(deltaTime);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }

        private void HandleLocomotion(float deltaTime)
        {
            _animator.PlayBottomLocomotion(_speed);
            _animator.PlayTopLocomotion(_speed);
        }

        private void HandleAggro(float deltaTime)
        {
            _animator.PlayBottomLocomotion(_speed);
            _animator.TryPlayAggroLocomotion(_speed);
        }

        private void HandleAggroIntro(float deltaTime)
        {
            _animator.PlayBottomLocomotion(_speed);
            _animator.TryPlayAggro();
        }

        private void HandleAttack(float deltaTime)
        {
            _animator.PlayBottomLocomotion(_speed);
            _animator.PlayAttack();
        }

        [Button("CalmDown")]
        private void CalmDown() => _animator.TryCalmDown();
    }
}
