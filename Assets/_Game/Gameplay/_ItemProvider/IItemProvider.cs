using _Game.Gameplay._RewardProcessing;

namespace _Game.Gameplay._ItemProvider
{
    public interface IItemProvider
    {
        ItemLocal GetItem(int id);
    }
}