using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class IdleWarriorState : WarriorStateBase
    {
        
        public IdleWarriorState(UnitBase unit, string name) : base(unit, name)
        {
            
        }

        public override void Enter()
        {
            _unit.AMove.Stop();
            _unit.CalmDown();
        }

        public override void GameUpdate(float deltaTime)
        {
            _unit.Animator.PlayBottomLocomotion(_unit.AMove.GetCurrentSpeed());
            _unit.Animator.PlayTopLocomotion(_unit.AMove.GetCurrentSpeed());
        }
    }
}