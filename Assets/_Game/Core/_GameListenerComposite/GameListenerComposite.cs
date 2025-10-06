using System;
using System.Collections.Generic;
using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay.BattleLauncher;
using _Game.UI.BattleResultPopup.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core._GameListenerComposite
{
    public class GameListenerComposite : 
        MonoBehaviour,
        IStartGameListener,
        IStopGameListener,
        IPauseListener,
        IEndGameListener,
        IGameSpeedListener,
        ISaveGameTrigger,
        IBattleChangeListener,
        IAgeChangeListener,
        ITimelineChangeListener,
        ILevelChangeListener
    {
        public event Action<bool> SaveGameRequested;

        [ShowInInspector]
        [Inject]
        private IGameManager _gameManager;

        [ShowInInspector]
        [Inject] 
        private List<IStartGameListener> _startBattleListeners = new();

        [ShowInInspector]
        [Inject] 
        private List<IPauseListener> _pauseListeners = new();

        [ShowInInspector]
        [Inject] 
        private List<IStopGameListener> _stopBattleListeners = new();

        [ShowInInspector]
        [Inject] 
        private List<IEndGameListener> _endBattleListeners = new();
        
        [ShowInInspector]
        [Inject]
        private IBattleSpeedManager _battleSpeedManager;

        [ShowInInspector]
        [Inject] 
        private List<IGameSpeedListener> _battleSpeedListeners = new();
        
        [Inject]
        private IMyLogger _logger;

        [ShowInInspector]
        [Inject]
        private IGameSaver _gameSaver;

        [ShowInInspector]
        [Inject] 
        private List<ISaveGameTrigger> _saveGameTriggers = new();
        
        [ShowInInspector]
        [Inject] 
        private List<IBattleChangeListener> _battleChangeListeners = new();
        
        [ShowInInspector]
        [Inject] 
        private List<IAgeChangeListener> _ageChangeListeners = new();

        [ShowInInspector]
        [Inject] 
        private List<ITimelineChangeListener> _timelineChangeListeners = new();
        
        [ShowInInspector]
        [Inject] 
        private List<ILevelChangeListener> _levelChangeListeners = new();
        
        private void Start()
        {
            if(_gameManager.IsPaused) _gameManager.SetPaused(false);
            
            foreach (var trigger in _saveGameTriggers) trigger.SaveGameRequested += OnSaveGameRequested;
            _gameManager.Register(this);
            _battleSpeedManager.Register(this);
            _gameSaver.Register(this);
        }

        private void OnDestroy()
        {
            foreach (var trigger in _saveGameTriggers) trigger.SaveGameRequested -= OnSaveGameRequested;
            _gameManager.Unregister(this);
            _battleSpeedManager.Unregister(this);
            _gameSaver.Unregister(this);
        }

        
        private void OnSaveGameRequested(bool isDebounced) => SaveGameRequested?.Invoke(isDebounced);

        void IStartGameListener.OnStartBattle()
        {
            foreach (var it in _startBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStartBattle();
                }
            }
        }

        void IPauseListener.SetPaused(bool isPaused)
        {
            foreach (var it in _pauseListeners)
            {
                if (it is { } listener)
                {
                    listener.SetPaused(isPaused);
                }
            }
        }

        void IStopGameListener.OnStopBattle()
        {
            foreach (var it in _stopBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnStopBattle();
                }
            }
        }

        void IEndGameListener.OnEndBattle(GameResultType result, bool wasExit)
        {
            foreach (var it in _endBattleListeners)
            {
                if (it is { } listener)
                {
                    listener.OnEndBattle(result, wasExit);
                }
            }
        }

        void IGameSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            foreach (var it in _battleSpeedListeners)
            {
                if (it is { } listener)
                {
                    listener.OnBattleSpeedFactorChanged(speedFactor);
                }
            }
        }
        
        void IBattleChangeListener.OnBattleChange(int battleIndex)
        {
            foreach (var it in _battleChangeListeners)
            {
                if (it is { } listener)
                {
                    listener.OnBattleChange(battleIndex);
                }
            }
        }

        void IAgeChangeListener.OnAgeChange(int age)
        {
            foreach (var it in _ageChangeListeners)
            {
                if (it is { } listener)
                {
                    listener.OnAgeChange(age);
                }
            }
        }

        void ITimelineChangeListener.OnTimelineChange(int timeline)
        {
            foreach (var it in _timelineChangeListeners)
            {
                if (it is { } listener)
                {
                    listener.OnTimelineChange(timeline);
                }
            }
        }

        void ILevelChangeListener.OnLevelChange(int level)
        {
            foreach (var it in _levelChangeListeners)
            {
                if (it is { } listener)
                {
                    listener.OnLevelChange(level);
                }
            }
        }
    }
}