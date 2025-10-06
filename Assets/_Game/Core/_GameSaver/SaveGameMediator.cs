using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core._GameSaver
{
    public class SaveGameMediator : ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;

        public void SaveGame(bool isDebounced)
        {
            SaveGameRequested?.Invoke(isDebounced);
        }
    }
}