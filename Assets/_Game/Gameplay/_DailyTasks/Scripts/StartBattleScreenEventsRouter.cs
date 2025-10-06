using System;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class StartBattleScreenEventsRouter : IGameScreenListener<IStartBattleScreen>
    {
        public event Action StartBattleScreenOpened;
        public event Action StartBattleScreenInfoChanged;
        public event Action StartBattleScreenRequiredAttention;
        public event Action StartBattleScreenClosed;
        public event Action StartBattleScreenDisposed;
        public event Action<bool> StartBattleScreenActiveChanged;

        void IGameScreenListener<IStartBattleScreen>.OnScreenOpened(IStartBattleScreen screen) => 
            StartBattleScreenOpened?.Invoke();

        void IGameScreenListener<IStartBattleScreen>.OnInfoChanged(IStartBattleScreen screen) => 
            StartBattleScreenInfoChanged?.Invoke();

        void IGameScreenListener<IStartBattleScreen>.OnRequiresAttention(IStartBattleScreen screen) => 
            StartBattleScreenRequiredAttention?.Invoke();

        void IGameScreenListener<IStartBattleScreen>.OnScreenClosed(IStartBattleScreen screen) => 
            StartBattleScreenClosed?.Invoke();

        void IGameScreenListener<IStartBattleScreen>.OnScreenActiveChanged(IStartBattleScreen screen, bool isActive) => 
            StartBattleScreenActiveChanged?.Invoke(isActive);

        void IGameScreenListener<IStartBattleScreen>.OnScreenDisposed(IStartBattleScreen screen) => 
            StartBattleScreenDisposed?.Invoke();
    }
}