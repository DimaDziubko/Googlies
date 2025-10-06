using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class TestMecanimUnit : MonoBehaviour
    {
        [SerializeField] private MecanimUnitAnimator _animator;
        [SerializeField] private Transform _target;
        private void Start()
        {
            //_animator.Construct(0.8f);
            _animator.PlayAttack();
            _animator.SetTarget(_target);
        }
    }
}
