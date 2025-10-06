using _Game.Core.UserState._State;

namespace _Game.Core.Services.UserContainer
{
    public class EventsStateHandler : IEventsStateHandler
    {
        private readonly UserContainer _userContainer;

        public EventsStateHandler(UserContainer userContainer) => 
            _userContainer = userContainer;

        public void AddActiveEvent(GameEventSavegame gameEvent)
        {
            _userContainer.State.EventsSavegame.AddActiveEvent(gameEvent);
            _userContainer.RequestSaveGame();
        }
        
        public void RemoveActiveEvent(GameEventSavegame gameEvent)
        {
            if(_userContainer.State.EventsSavegame.RemoveActiveEvent(gameEvent))
                _userContainer.RequestSaveGame();
        }

        public void AddPastEvent(GameEventSavegame gameEvent)
        {
            _userContainer.State.EventsSavegame.AddPastEvent(gameEvent);
            _userContainer.RequestSaveGame();
        }
    
        public void RemovePastEvent(GameEventSavegame gameEvent)
        {
            if(_userContainer.State.EventsSavegame.RemovePastEvent(gameEvent))
                _userContainer.RequestSaveGame();
        }
    
        public void MoveToPast(GameEventSavegame gameEvent)
        {
            _userContainer.State.EventsSavegame.MoveToPast(gameEvent);
            _userContainer.RequestSaveGame();
        }
    }
}