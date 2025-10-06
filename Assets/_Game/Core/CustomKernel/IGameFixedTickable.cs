namespace _Game.Core.CustomKernel
{
    public interface IGameFixedTickable
    {
        void FixedTick(float deltaTime);
    }
}