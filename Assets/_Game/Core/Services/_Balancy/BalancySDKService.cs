using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Utils;
using Balancy;
using Balancy.Data;
using Balancy.Data.SmartObjects;
using Balancy.Models;
using Zenject;
using Constants = Balancy.Constants;

namespace _Game.Core.Services._Balancy
{
    public interface IBalancySDKService
    {
        event Action Initialized;
        event Action ProfileLoaded;
        bool IsInitialized { get;}
        EventInfo[] GetActiveEvents();
        EventIAPs[] GetEventIAPs();
        IEnumerable<CustomGameEvent> GetAllEvents();
        GameProfile GetProfile();
        void LoadProfile();
        void ResetProfile();
    }

    public class BalancySDKService : 
        IInitializable, 
        IDisposable, 
        IBalancySDKService
    {
        public event Action Initialized;
        public event Action ProfileLoaded;

        private readonly IMyLogger _logger;
        private readonly BalancySettings _settings;
        private GameProfile _profile;

        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public BalancySDKService(
            IMyLogger logger,
            BalancySettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        void IInitializable.Initialize()
        {
//            var perfTimer = new PerfTimer(_logger, "Balancy initialization");
//            perfTimer.Start();
            
//            Main.Init(new AppConfig
//            {
//                ApiGameId = _settings.GameID,
//                PublicKey = _settings.PublicKey,
//                Environment = _settings.Environment,
//                AutoLogin = true,
//                OfflineMode = false,
//#if UNITY_ANDROID
//                Platform = Constants.Platform.AndroidGooglePlay,
//#elif UNITY_IOS
//                Platform = Constants.Platform.IosAppStore,
//#endif
//                PreInit = PreInitType.None,
//                OnInitProgress = progress =>
//                {
//                    _logger.Log($"***=> STATUS {progress.Status}", DebugStatus.Info);
//                    switch (progress.Status)
//                    {
//                        case BalancyInitStatus.PreInitFromResourcesOrCache:
//                            break;
//                        case BalancyInitStatus.PreInitLocalProfile:
//                            break;
//                        case BalancyInitStatus.DictionariesReady:
//                            break;
//                        case BalancyInitStatus.Finished:
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                },
//                UpdateType = UpdateType.FullUpdate,
//                UpdatePeriod = 300,
//                OnContentUpdateCallback = updateResponse =>
//                {
//                    _logger.Log("Content Updated " + updateResponse.AffectedDictionaries.Length,
//                        DebugStatus.Success);
//                },
//                OnReadyCallback = response =>
//                {
//                    _logger.Log($"Balancy Init Complete: {response.Success}, deploy version = {response.DeployVersion}",
//                        DebugStatus.Success);
                    
//                    _isInitialized = true;
//                    Initialized?.Invoke();

//                    perfTimer.Stop();
//                }
//            });
        }

        public void LoadProfile()
        {
            SmartStorage.LoadSmartObject<GameProfile>(responseData =>
            {
                _profile = responseData.Data;
                ProfileLoaded?.Invoke();
                _logger.Log("PROFILE LOADED", DebugStatus.Info);
            });
        }

        public EventInfo[] GetActiveEvents()
        {
            if (!_isInitialized) return new EventInfo[] { };
            return LiveOps.GameEvents.GetActiveEvents();
        }

        public EventIAPs[] GetEventIAPs()
        {
            if (!_isInitialized) return new EventIAPs[] { };
            return DataEditor.EventIAPs.ToArray();
        }

        public IEnumerable<CustomGameEvent> GetAllEvents()
        {
            if (!_isInitialized) return new CustomGameEvent[] { };
            return DataEditor.CustomGameEvents.ToArray();
        }

        public GameProfile GetProfile()
        {
            if (!_isInitialized) return null;
            return _profile;
        }

        public void ResetProfile() => 
            LiveOps.Profile.Restart(LoadProfile);

        void IDisposable.Dispose() { }
    }
}