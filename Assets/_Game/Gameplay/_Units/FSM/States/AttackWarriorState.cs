using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class AttackWarriorState : WarriorStateBase
    {
        public AttackWarriorState(UnitBase unit, string name) : base(unit, name)
        {
            _unit.TargetDetection.AttackTargetChanged += OnUpdatedTarget;
            
            _unit.Pusher.OnUnitPushedOut += OnPushedOut;
        }

        public override void Enter()
        {
            if (!_unit.Pusher.IsInsideBase) 
                _unit.AMove.Stop();
        }

        public override void GameUpdate(float deltaTime)
        {
            _unit.Animator.PlayAttack();
            _unit.Animator.TryStartAiming();
            _unit.Animator.PlayBottomLocomotion(_unit.AMove.GetCurrentSpeed());
        }

        public override void Exit() { }

        private void OnPushedOut() => _unit.AMove.Stop();

        private void OnUpdatedTarget(ITarget target)
        {
            if(target == null) return;

            _unit.Weapon.SetTarget(target);
            _unit.Animator.SetTarget(target.Transform);
        }

        public override void Cleanup()
        {
            _unit.TargetDetection.AttackTargetChanged -= OnUpdatedTarget;
            _unit.Pusher.OnUnitPushedOut -= OnPushedOut;
        }
    }
}