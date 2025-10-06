namespace _Game.UI._MainMenu.State
{
    public interface ILocalPayloadedState<TPayload> : ILocalExitableState
    {
        void Enter(TPayload destination);
    }
}