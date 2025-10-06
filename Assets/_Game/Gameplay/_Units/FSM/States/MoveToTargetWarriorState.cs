using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class MoveToTargetWarriorState : WarriorStateBase
    {
        public MoveToTargetWarriorState(UnitBase unit, string name) : base(unit, name)
        {
        }
        
        public override void Enter()
        {
            _unit.Animator.TryPlayAggro();
            _unit.TargetDetection.OnPersecutionTargetChanged += OnTargetChanged;
        }

        public override void GameUpdate(float deltaTime)
        {
            if (_unit.TargetDetection.TryGetTargetForPersecution(out ITarget target))
            {
                _unit.AMove.Move(target.Transform.position);
            };
            
            _unit.Animator.PlayBottomLocomotion(_unit.AMove.GetCurrentSpeed());
            _unit.Animator.TryPlayAggroLocomotion(_unit.AMove.GetCurrentSpeed());
        }

        public override void Exit()
        {
            _unit.TargetDetection.OnPersecutionTargetChanged -= OnTargetChanged;
            //_unit.Animator.TryStopAiming();
        }

        private void OnTargetChanged(ITarget target)
        {
            if(target != null)
                _unit.AMove.Move(target.Transform.position);
        }
    }
}