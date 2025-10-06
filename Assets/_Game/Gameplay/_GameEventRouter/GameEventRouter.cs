using System;
using _Game.Core._GameListenerComposite;
using _Game.UI.BattleResultPopup.Scripts;
using Sirenix.OdinInspector;

namespace _Game.Gameplay._GameEventRouter
{
    public class GameEventRouter :
        IAgeChangeListener,
        IBattleChangeListener,
        ITimelineChangeListener,
        ILevelChangeListener,
        IStartGameListener,
        IStopGameListener,
        IEndGameListener
    {
        public event Action<int> TimelineChanged;
        public event Action<int> AgeChanged;
        public event Action<int> BattleChanged;
        public event Action<int> LevelChanged;
        
        public event Action BattleStarted;
        public event Action BattleStopped;
        public event Action<GameResultType, bool> BattleEnded;

        [Button]
        void ITimelineChangeListener.OnTimelineChange(int timeline)
        {
            TimelineChanged?.Invoke(timeline);
        }

        [Button]
        void IAgeChangeListener.OnAgeChange(int age)
        {
            AgeChanged?.Invoke(age);
        }

        [Button]
        void IBattleChangeListener.OnBattleChange(int battleIndex)
        {
            BattleChanged?.Invoke(battleIndex);
        }
        
        [Button]
        void ILevelChangeListener.OnLevelChange(int level)
        {
            LevelChanged?.Invoke(level);
        }

        void IStartGameListener.OnStartBattle()
        {
            BattleStarted?.Invoke();
        }

        void IStopGameListener.OnStopBattle()
        {
            BattleStopped?.Invoke();
        }

        void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit)
        {
            BattleEnded?.Invoke(result, wasExit);
        }
    }
}