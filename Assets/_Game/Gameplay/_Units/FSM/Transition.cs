namespace _Game.Gameplay._Units.FSM
{
    public class Transition : ITransition
    {
        public IWarriorState To { get;}
        public IPredicate Condition { get; }

        public Transition(
            IWarriorState to,
            IPredicate condition)
        {
            Condition = condition;
            To = to;
        }
    }
}