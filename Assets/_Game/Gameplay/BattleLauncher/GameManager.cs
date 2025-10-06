using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.UI.BattleResultPopup.Scripts;

namespace _Game.Gameplay.BattleLauncher
{
    public enum BattleState
    {
        None,
        Play,
        Stop,
        End
    }
    
    public sealed  class GameManager : IGameManager
    {
        private readonly List<IGameListener> _listeners = new();
        private readonly IMyLogger _logger;

        public BattleState State { get; private set; }

        public bool IsPaused { get; private set; }

        public GameManager(
            IMyLogger logger)
        {
            State = BattleState.None;
            _logger = logger;
        }
        
        public void Register(IGameListener listener) => 
            _listeners.Add(listener);

        public void Unregister(IGameListener listener) => 
            _listeners.Remove(listener);

        public void StartBattle()
        {
            State = BattleState.Play;
            _logger.Log("MANAGER START BATTLE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is IStartGameListener startBattleListener)
                {
                    startBattleListener.OnStartBattle();
                }
            }
        }

        public void SetPaused(bool isPaused)
        {
            IsPaused = isPaused;
            _logger.Log($"MANAGER PAUSE BATTLE {isPaused}");
            
            foreach (var it in _listeners)
            {
                if (it is IPauseListener pauseListener)
                {
                    pauseListener.SetPaused(isPaused);
                }
            }
        }

        public void StopBattle()
        {
            State = BattleState.Stop;
            
            _logger.Log("MANAGER STOP BATTLE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is IStopGameListener stopListener)
                {
                    stopListener.OnStopBattle();
                }
            }
        }
        
        public void EndBattle(GameResultType result, bool wasExit)
        {
            State = BattleState.End;
            _logger.Log("MANAGER END BATTLE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is IEndGameListener endListener)
                {
                    endListener.OnEndBattle(result, wasExit);
                }
            }
        }
        
        public void ChangeBattle(int battleIdx)
        {
            _logger.Log("MANAGER CHANGE BATTLE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is IBattleChangeListener battleChangeListener)
                {
                    battleChangeListener.OnBattleChange(battleIdx);
                }
            }
        }

        public void ChangeAge(int ageIdx)
        {
            _logger.Log("MANAGER CHANGE AGE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is IAgeChangeListener ageChangeListener)
                {
                    ageChangeListener.OnAgeChange(ageIdx);
                }
            }
        }

        public void ChangeTimeline(int timelineId)
        {
            _logger.Log("MANAGER CHANGE TIMELINE", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is ITimelineChangeListener timelineChangeListener)
                {
                    timelineChangeListener.OnTimelineChange(timelineId);
                }
            }
        }

        public void ChangeLevel(int level)
        {
            _logger.Log("MANAGER CHANGE LEVEL", DebugStatus.Info);
            
            foreach (var it in _listeners)
            {
                if (it is ILevelChangeListener levelChangeListener)
                {
                    levelChangeListener.OnLevelChange(level);
                }
            }
        }
        
    }
}