namespace _Game.Core.CustomKernel
{
    public interface IGameLateTickable
    {
        void LateTick(float deltaTime);
    }
}