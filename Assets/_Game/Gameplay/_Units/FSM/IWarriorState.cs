namespace _Game.Gameplay._Units.FSM
{
    public interface IWarriorState
    {
        string Name { get; }
        void Enter();
        void GameUpdate(float deltaTime);
        void FixedUpdate();
        void Exit();
        void Cleanup();
    }
}