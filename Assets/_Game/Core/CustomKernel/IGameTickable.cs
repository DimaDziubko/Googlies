namespace _Game.Core.CustomKernel
{
    public interface IGameTickable
    {
        void Tick(float deltaTime);
    }
}