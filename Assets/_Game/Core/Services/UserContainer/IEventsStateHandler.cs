using _Game.Core.UserState._State;

namespace _Game.Core.Services.UserContainer
{
    public interface IEventsStateHandler
    {
        void AddActiveEvent(GameEventSavegame gameEvent);
        void RemoveActiveEvent(GameEventSavegame gameEvent);
        void AddPastEvent(GameEventSavegame gameEvent);
        void RemovePastEvent(GameEventSavegame gameEvent);
        void MoveToPast(GameEventSavegame gameEvent);
    }
}