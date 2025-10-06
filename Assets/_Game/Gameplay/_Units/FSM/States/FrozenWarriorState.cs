using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class FrozenWarriorState : WarriorStateBase
    {
        public FrozenWarriorState(UnitBase unit, string name) : base(unit, name)
        {
        }
        public override void Enter()
        {
            _unit.AMove.Stop();
            _unit.Animator.SetSpeedFactor(0);
        }

        public override void Exit() => _unit.Animator.ResetSpeed();
    }
}