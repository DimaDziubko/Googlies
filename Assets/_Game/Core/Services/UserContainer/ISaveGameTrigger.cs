using System;

namespace _Game.Core.Services.UserContainer
{
    public interface ISaveGameTrigger
    {
        event Action<bool> SaveGameRequested;
    }
}