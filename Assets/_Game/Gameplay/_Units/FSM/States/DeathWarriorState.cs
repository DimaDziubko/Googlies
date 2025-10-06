using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public class DeathWarriorState : WarriorStateBase
    {
        public DeathWarriorState(UnitBase unit, string name) : base(unit, name)
        {
            
        }

        public override void Enter()
        {
            _unit.AMove.Stop();
            _unit.CalmDown();
            _unit.HandleDeath();
        }
    }
}