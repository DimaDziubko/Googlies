using System;
using _Game.Core._GameMode;
using _Game.Core._Logger;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Core.Configs.Providers
{
    public interface IRemoteConfigProvider
    {
        UniTask<JObject> GetExtraConfig();
        void ClearCache();
        UniTask<JObject> GetDungeonConfig();
        UniTask<bool> GetIsShowForcedUpdate();
        UniTask<JObject> GetAgesConfig();
        UniTask<JObject> GetBattlesConfig();
        UniTask<JObject> GetWeaponsConfig();
        UniTask<JObject> GetWarriorsConfig();
        UniTask<JObject> GetSkillsConfig();
    }

    public class RemoteConfigProvider : IRemoteConfigProvider
    {
        private readonly IMyLogger _logger;

        private JObject _cachedConfig;
        private JObject _cachedDungeonConfig;
        private JObject _cachedAgesConfig;
        private JObject _cachedBattlesConfig;
        private JObject _cachedWarriorsConfig;
        private JObject _cachedWeaponsConfig;
        private JObject _cachedSkillsConfig;

        private bool _isConfigLoaded;
        private bool _isDungeonConfigLoaded;
        private bool _isAgesConfigLoaded;
        private bool _isBattlesConfigLoaded;
        private bool _isWarriorsConfigLoaded;
        private bool _isWeaponsConfigLoaded;
        private bool _isSkillsConfigLoaded;

        public RemoteConfigProvider(IMyLogger logger)
        {
            _logger = logger;
        }

        public async UniTask<JObject> GetExtraConfig()
        {
            if (!_isConfigLoaded)
            {
                await LoadConfig("ExtraConfig", "ExtraConfigTest", config => _cachedConfig = config);
                _isConfigLoaded = true;
            }

            return _cachedConfig;
        }

        public async UniTask<JObject> GetDungeonConfig()
        {
            if (!_isDungeonConfigLoaded)
            {
                await LoadConfig("Dungeons", "DungeonsTest", config => _cachedDungeonConfig = config);
                _isDungeonConfigLoaded = true;
            }

            return _cachedDungeonConfig;
        }
        
        public async UniTask<JObject> GetSkillsConfig()
        {
            if (!_isSkillsConfigLoaded)
            {
                await LoadConfig("Skills", "SkillsTest", config => _cachedSkillsConfig = config);
                _isSkillsConfigLoaded = true;
            }

            return _cachedSkillsConfig;
        }
        
        public async UniTask<bool> GetIsShowForcedUpdate()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("Internet connection is not available.");
                return false;
            }

            bool isEnabled = false;

            await LoadConfig("ForcedUpdate", "ForcedUpdateTest", config =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(config))
                    {
                        if (bool.TryParse(config, out var parsedValue))
                        {
                            isEnabled = parsedValue;
                        }
                        else
                        {
                            _logger.Log("Failed to parse config string to boolean.", DebugStatus.Warning);
                        }
                    }
                    else
                    {
                        _logger.Log("Config string is null or empty.", DebugStatus.Warning);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error parsing config string: {ex.Message}", DebugStatus.Fault);
                }
            });

            return isEnabled;
        }

        public async UniTask<JObject> GetAgesConfig()
        {
            if (!_isAgesConfigLoaded)
            {
                await LoadConfig("Ages", "AgesTest", config => _cachedAgesConfig = config);
                _isAgesConfigLoaded = true;
            }

            return _cachedAgesConfig;
        }

        public async UniTask<JObject> GetBattlesConfig()
        {
            if (!_isBattlesConfigLoaded)
            {
                await LoadConfig("Battles", "BattlesTest", config => _cachedBattlesConfig = config);
                _isBattlesConfigLoaded = true;
            }

            return _cachedBattlesConfig;
        }

        public async UniTask<JObject> GetWarriorsConfig()
        {
            if (!_isWarriorsConfigLoaded)
            {
                await LoadConfig("Warriors", "WarriorsTest", config => _cachedWarriorsConfig = config);
                _isWarriorsConfigLoaded = true;
            }

            return _cachedWarriorsConfig;
        }

        public async UniTask<JObject> GetWeaponsConfig()
        {
            if (!_isWeaponsConfigLoaded)
            {
                await LoadConfig("Weapons", "WeaponsTest", config => _cachedWeaponsConfig = config);
                _isWeaponsConfigLoaded = true;
            }

            return _cachedWeaponsConfig;
        }

        public void ClearCache()
        {
            _cachedConfig = null;
            _cachedDungeonConfig = null;
            _cachedAgesConfig = null;
            _cachedBattlesConfig = null;
            _cachedWarriorsConfig = null;
            _cachedSkillsConfig = null;

            _isConfigLoaded = false;
            _isDungeonConfigLoaded = false;
            _isAgesConfigLoaded = false;
            _isBattlesConfigLoaded = false;
            _isWarriorsConfigLoaded = false;
            _isSkillsConfigLoaded = false;
        }

        private async UniTask LoadConfig(string prodKey, string testKey, Action<JObject> cacheSetter)
        {
            _logger.Log($"Fetching data for {prodKey} or {testKey}...");
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

                var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                var info = remoteConfig.Info;

                if (info.LastFetchStatus != LastFetchStatus.Success)
                {
                    _logger.LogError($"Fetch was unsuccessful\nLastFetchStatus: {info.LastFetchStatus}");
                    return;
                }

                await remoteConfig.ActivateAsync();
                _logger.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}");

                string configString = GameModeSettings.I.TestMode
                    ? remoteConfig.GetValue(testKey).StringValue
                    : remoteConfig.GetValue(prodKey).StringValue;

                _logger.Log(GameModeSettings.I.TestMode ? $"{testKey} loaded" : $"{prodKey} loaded");
                _logger.Log(configString);

                var configJsonData = JObject.Parse(configString);
                cacheSetter(configJsonData);
            }
            catch (Exception e)
            {
                _logger.Log($"Error fetching remote config: {e.Message}");
            }
        }
        private async UniTask LoadConfig(string prodKey, string testKey, Action<string> cacheSetter)
        {
            _logger.Log($"Fetching data for {prodKey} or {testKey}...");
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);

                var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                var info = remoteConfig.Info;

                if (info.LastFetchStatus != LastFetchStatus.Success)
                {
                    _logger.Log($"Fetch was unsuccessful\nLastFetchStatus: {info.LastFetchStatus}", DebugStatus.Fault);
                    return;
                }

                await remoteConfig.ActivateAsync();
                _logger.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}");

                string configString = GameModeSettings.I.TestMode
                    ? remoteConfig.GetValue(testKey).StringValue
                    : remoteConfig.GetValue(prodKey).StringValue;

                _logger.Log(GameModeSettings.I.TestMode ? $"{testKey} loaded" : $"{prodKey} loaded");
                _logger.Log(configString);

                cacheSetter(configString);
            }
            catch (Exception e)
            {
                _logger.Log($"Error fetching remote config: {e.Message}");
            }
        }
    }
}