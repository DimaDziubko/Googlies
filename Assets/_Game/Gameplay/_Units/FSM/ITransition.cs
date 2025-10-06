namespace _Game.Gameplay._Units.FSM
{
    public interface ITransition
    {
        IWarriorState To { get; }
        IPredicate Condition { get; }
    }
}