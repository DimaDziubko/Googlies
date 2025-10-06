using _Game.Core.Services.UserContainer;

namespace _Game.Core._GameSaver
{
    public interface IGameSaver
    {
        void Register(ISaveGameTrigger trigger);
        void Unregister(ISaveGameTrigger trigger);
    }
}