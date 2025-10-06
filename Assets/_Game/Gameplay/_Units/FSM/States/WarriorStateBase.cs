using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._Units.FSM.States
{
    public abstract class WarriorStateBase : IWarriorState
    {
        public string Name { get; set; }
        
        protected readonly UnitBase _unit;
        
        protected WarriorStateBase(UnitBase unit, string name)
        {
            _unit = unit;
            Name = name;
        }

        public virtual void Enter()
        {
            //noop
        }

        public virtual void GameUpdate(float deltaTime)
        {
            //noop
        }

        public virtual void FixedUpdate()
        {
            //noop
        }

        public virtual void Exit()
        {
            //noop
        }

        public virtual void Cleanup()
        {
            //noop
        }
    }
}