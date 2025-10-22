using System;
using _Game.Core._GameMode;
using _Game.Core._Logger;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;

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

        // Общее состояние для Firebase
        private static bool _isFirebaseInitialized = false;
        private static bool _isFetching = false;
        private static UniTaskCompletionSource _fetchCompletionSource;
        private static bool _fetchSuccessful = false;
        private static DateTime _lastSuccessfulFetch = DateTime.MinValue;
        private static readonly TimeSpan FETCH_COOLDOWN = TimeSpan.FromMinutes(1); // Минимум минута между fetch'ами

        public RemoteConfigProvider(IMyLogger logger)
        {
            _logger = logger;
        }

        public async UniTask<JObject> GetExtraConfig()
        {
            if (!_isConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedConfig = await LoadConfigSafe("ExtraConfig", "ExtraConfigTest");
                _isConfigLoaded = true;
            }

            return _cachedConfig;
        }

        public async UniTask<JObject> GetDungeonConfig()
        {
            if (!_isDungeonConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedDungeonConfig = await LoadConfigSafe("Dungeons", "DungeonsTest");
                _isDungeonConfigLoaded = true;
            }

            return _cachedDungeonConfig;
        }

        public async UniTask<JObject> GetSkillsConfig()
        {
            if (!_isSkillsConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedSkillsConfig = await LoadConfigSafe("Skills", "SkillsTest");
                _isSkillsConfigLoaded = true;
            }

            return _cachedSkillsConfig;
        }

        public async UniTask<bool> GetIsShowForcedUpdate()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _logger.Log("Internet connection is not available.", DebugStatus.Warning);
                return false;
            }

            await EnsureFirebaseReady();

            try
            {
                var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                string configString = GameModeSettings.I.TestMode
                    ? remoteConfig.GetValue("ForcedUpdateTest").StringValue
                    : remoteConfig.GetValue("ForcedUpdate").StringValue;

                if (string.IsNullOrEmpty(configString))
                {
                    _logger.Log("ForcedUpdate config is null or empty.", DebugStatus.Warning);
                    return false;
                }

                if (bool.TryParse(configString, out var result))
                {
                    return result;
                }

                _logger.Log($"Failed to parse ForcedUpdate config: {configString}", DebugStatus.Warning);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error getting ForcedUpdate config: {ex.Message}", DebugStatus.Fault);
                return false;
            }
        }

        public async UniTask<JObject> GetAgesConfig()
        {
            if (!_isAgesConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedAgesConfig = await LoadConfigSafe("Ages", "AgesTest");
                _isAgesConfigLoaded = true;
            }

            return _cachedAgesConfig;
        }

        public async UniTask<JObject> GetBattlesConfig()
        {
            if (!_isBattlesConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedBattlesConfig = await LoadConfigSafe("Battles", "BattlesTest");
                _isBattlesConfigLoaded = true;
            }

            return _cachedBattlesConfig;
        }

        public async UniTask<JObject> GetWarriorsConfig()
        {
            if (!_isWarriorsConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedWarriorsConfig = await LoadConfigSafe("Warriors", "WarriorsTest");
                _isWarriorsConfigLoaded = true;
            }

            return _cachedWarriorsConfig;
        }

        public async UniTask<JObject> GetWeaponsConfig()
        {
            if (!_isWeaponsConfigLoaded)
            {
                await EnsureFirebaseReady();
                _cachedWeaponsConfig = await LoadConfigSafe("Weapons", "WeaponsTest");
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
            _cachedWeaponsConfig = null;

            _isConfigLoaded = false;
            _isDungeonConfigLoaded = false;
            _isAgesConfigLoaded = false;
            _isBattlesConfigLoaded = false;
            _isWarriorsConfigLoaded = false;
            _isSkillsConfigLoaded = false;

            // Сбрасываем состояние Firebase для повторного fetch
            _isFirebaseInitialized = false;
            _fetchSuccessful = false;
            _fetchCompletionSource = null;
            _isFetching = false;
        }

        /// <summary>
        /// Обеспечивает единократную загрузку данных из Firebase
        /// </summary>
        private async UniTask EnsureFirebaseReady()
        {
            if (_isFirebaseInitialized && _fetchSuccessful)
            {
                return;
            }

            // Если уже идет fetch, ждем его завершения
            if (_isFetching)
            {
                if (_fetchCompletionSource != null)
                {
                    await _fetchCompletionSource.Task;
                }
                return;
            }

            // Проверяем cooldown
            if (_lastSuccessfulFetch != DateTime.MinValue &&
                DateTime.Now - _lastSuccessfulFetch < FETCH_COOLDOWN)
            {
                _logger.Log("Firebase fetch is on cooldown, using cached data.");
                _isFirebaseInitialized = true;
                return;
            }

            _isFetching = true;
            _fetchCompletionSource = new UniTaskCompletionSource();

            try
            {
                await FetchRemoteConfigOnce();
                _fetchCompletionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                _fetchCompletionSource.TrySetException(ex);
            }
            finally
            {
                _isFetching = false;
                _fetchCompletionSource = null;
            }
        }

        /// <summary>
        /// Единократный fetch конфигурации из Firebase
        /// </summary>
        private async UniTask FetchRemoteConfigOnce()
        {
            if (_isFirebaseInitialized && _fetchSuccessful)
            {
                return;
            }

            _logger.Log("[FirebaseRemoteConfig] Starting fetch...");

            try
            {
                var remoteConfig = FirebaseRemoteConfig.DefaultInstance;

                // Fetch с таймаутом
                await remoteConfig.FetchAsync(TimeSpan.Zero);

                var info = remoteConfig.Info;

                if (info.LastFetchStatus != LastFetchStatus.Success)
                {
                    _logger.Log($"[FirebaseRemoteConfig] Fetch failed. Status: {info.LastFetchStatus}", DebugStatus.Warning);

                    // Даже если fetch не удался, можем использовать закешированные данные
                    _isFirebaseInitialized = true;
                    _fetchSuccessful = false;
                    return;
                }

                await remoteConfig.ActivateAsync();

                _logger.Log($"[FirebaseRemoteConfig] Remote data loaded successfully. Last fetch time: {info.FetchTime}");

                _isFirebaseInitialized = true;
                _fetchSuccessful = true;
                _lastSuccessfulFetch = DateTime.Now;
            }
            catch (Exception e)
            {
                _logger.Log($"[FirebaseRemoteConfig] Error during fetch: {e.Message}", DebugStatus.Warning);

                // Помечаем как инициализированный, чтобы использовать fallback данные
                _isFirebaseInitialized = true;
                _fetchSuccessful = false;
            }
        }

        /// <summary>
        /// Безопасная загрузка конфига с fallback на embedded данные
        /// </summary>
        private async UniTask<JObject> LoadConfigSafe(string prodKey, string testKey)
        {
            try
            {
                var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                string configKey = GameModeSettings.I.TestMode ? testKey : prodKey;
                string configString = remoteConfig.GetValue(configKey).StringValue;

                if (string.IsNullOrEmpty(configString))
                {
                    _logger.Log($"Remote config for {configKey} is empty, trying fallback...", DebugStatus.Warning);
                    return await LoadFallbackConfig(configKey);
                }

                _logger.Log($"{configKey} loaded from remote config");
                return JObject.Parse(configString);
            }
            catch (Exception e)
            {
                _logger.Log($"Error parsing remote config for {prodKey}/{testKey}: {e.Message}", DebugStatus.Warning);
                return await LoadFallbackConfig(GameModeSettings.I.TestMode ? testKey : prodKey);
            }
        }

        /// <summary>
        /// Загрузка embedded конфига как fallback
        /// </summary>
        private async UniTask<JObject> LoadFallbackConfig(string configName)
        {
            try
            {
                _logger.Log($"Loading embedded config as fallback for {configName}.", DebugStatus.Warning);

                // Здесь должна быть логика загрузки embedded конфига
                // Например, из Resources или StreamingAssets
                string fallbackPath = $"Configs/{configName}";
                TextAsset fallbackAsset = Resources.Load<TextAsset>(fallbackPath);

                if (fallbackAsset == null)
                {
                    _logger.Log($"Fallback config not found at {fallbackPath}", DebugStatus.Fault);
                    return new JObject(); // Возвращаем пустой объект вместо null
                }

                string fallbackJson = fallbackAsset.text;

                if (string.IsNullOrEmpty(fallbackJson))
                {
                    _logger.Log($"Fallback config is empty for {configName}", DebugStatus.Fault);
                    return new JObject();
                }

                return JObject.Parse(fallbackJson);
            }
            catch (Exception e)
            {
                _logger.Log($"Error loading fallback config for {configName}: {e.Message}", DebugStatus.Fault);
                return new JObject();
            }
        }
    }
}