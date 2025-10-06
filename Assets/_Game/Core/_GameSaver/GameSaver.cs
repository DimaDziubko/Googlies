using System;
using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Communication;
using _Game.Core.Services.UserContainer;
using ImprovedTimers;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace _Game.Core._GameSaver
{
    public class GameSaver : 
        IGameSaver,
        IStopGameListener,
        IDisposable,
        IInitializable
    {
        private const float AUTO_SAVE_TIME_SECONDS = 300f;
        
        private readonly IUserStateCommunicator _communicator;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        private readonly List<ISaveGameTrigger> _triggers = new();

        private readonly float _debounceTime = 1.0f;
        private float _lastSaveTime;

        private CountdownTimer _autoSaveTimer;
        private CountdownTimer _debounceSaveTimer;

        public GameSaver(
            IUserStateCommunicator communicator,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _communicator = communicator;
            _userContainer = userContainer;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            _autoSaveTimer = new CountdownTimer(AUTO_SAVE_TIME_SECONDS);
            _autoSaveTimer.TimerStop += OnAutoSaveStopped;
            _autoSaveTimer.Start();
            
            _debounceSaveTimer = new CountdownTimer(_debounceTime);
            _debounceSaveTimer.TimerStop += SaveGame; 
            
            Application.quitting += OnApplicationQuit;
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }
        
        private void OnAutoSaveStopped()
        {
            SaveGame();
            _autoSaveTimer.Reset();
            _autoSaveTimer.Start();
            _logger.Log("Auto save", DebugStatus.Success);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
            _logger.Log("Quit save", DebugStatus.Success);
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                SaveGame();
            }
        }
#endif
        
        public void Register(ISaveGameTrigger trigger)
        {
            trigger.SaveGameRequested += SaveGameRequested;
            _triggers.Add(trigger);
        }

        public void Unregister(ISaveGameTrigger trigger)
        {
            trigger.SaveGameRequested -= SaveGameRequested;
            _triggers.Remove(trigger);
        }

        void IStopGameListener.OnStopBattle() => SaveGame();

        private void SaveGameRequested(bool isDebounced)
        {
            if(isDebounced) ResetDebounceSaveGame();
            else
                SaveGame();
        }
        
        private void ResetDebounceSaveGame()
        {
            _debounceSaveTimer.Reset();
            
            if(!_debounceSaveTimer.IsRunning)
                _debounceSaveTimer.Start();
        
            _logger.Log("Debounce save timer reset", DebugStatus.Success);
        }
        
        private void SaveGame()
        {
            _communicator.SaveUserState(_userContainer.State);
            _logger.Log("SaveGame", DebugStatus.Success);
        }


        void IDisposable.Dispose()
        {
            Application.quitting -= OnApplicationQuit;
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
            
            _autoSaveTimer.Stop();
            _autoSaveTimer.Dispose();
        }
    }
}