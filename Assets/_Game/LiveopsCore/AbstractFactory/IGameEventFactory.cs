using _Game.Core.UserState._State;
using Balancy.Data.SmartObjects;

namespace _Game.LiveopsCore.AbstractFactory
{
    public interface IGameEventFactory
    {
        GameEventBase CreateModel(EventInfo remote, GameEventSavegame save, bool isNew);
        GameEventBase CreateModel(GameEventSavegame save);
        GameEventBase TryCreateWithPendingRewards(GameEventSavegame save);
    }
}