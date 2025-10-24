using System;
using _Game.Core._GameMode;
using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._GameConfig;
using _Game.Core.Configs.Providers;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class ConfigOperation : ILoadingOperation
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly ForcedUpdateService _forcedUpdateService;

        public string Description => "Configuration...";

        public ConfigOperation(
            IUserContainer userContainer,
            IMyLogger logger,
            ForcedUpdateService forcedUpdateService)
        {
            _userContainer = userContainer;
            _logger = logger;
            _forcedUpdateService = forcedUpdateService;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            _userContainer.GameConfig = await LoadGameConfig(onProgress);
            onProgress?.Invoke(1f);
#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }

        private async UniTask<GameConfig> LoadGameConfig(Action<float> onProgress)
        {
            // Теперь все UniTask.WhenAll вызовы будут работать с уже инициализированным Firebase
            var localConfigContainer =
                Resources.Load<GameLocalConfigContainer>(Constants.LocalConfigPath.GAME_LOCAL_CONFIG_CONTAINER_PATH);

            IRemoteConfigProvider remoteConfigProvider = new RemoteConfigProvider(_logger);
            // Инициализируем Firebase один раз перед всеми параллельными вызовами
            // await remoteConfigProvider.EnsureFirebaseReady(); // нужно сделать этот метод public
            ILocalConfigProvider localConfigProvider = new LocalConfigProvider();
            EmbeddedConfigProvider embeddedConfigProvider = new EmbeddedConfigProvider();

            ExtraConfigMapper extraConfigMapper = new ExtraConfigMapper(_logger);
            DungeonConfigMapper dungeonConfigMapper = new DungeonConfigMapper(_logger);

            AgesConfigMapper agesConfigMapper = new AgesConfigMapper(_logger);
            BattlesConfigMapper battlesConfigMapper = new BattlesConfigMapper(_logger);
            WarriorsConfigMapper warriorsConfigMapper = new WarriorsConfigMapper(_logger);
            WeaponsConfigMapper weaponsConfigMapper = new WeaponsConfigMapper(_logger);
            SkillsConfigMapper skillsConfigMapper = new SkillsConfigMapper(_logger);

            GameConfig config = new();

            try
            {
                onProgress?.Invoke(0.0f);

                (JObject, JObject, JObject, JObject, JObject, JObject, JObject) configs = await UniTask.WhenAll(
                    TryLoadConfig(remoteConfigProvider.GetExtraConfig, localConfigProvider.GetConfig, embeddedConfigProvider.LoadGameConfig),
                    TryLoadConfig(remoteConfigProvider.GetDungeonConfig, localConfigProvider.GetDungeonConfig, embeddedConfigProvider.LoadDungeonsConfig),
                    TryLoadConfig(remoteConfigProvider.GetAgesConfig, localConfigProvider.GetAgesConfig, embeddedConfigProvider.LoadAgesConfig),
                    TryLoadConfig(remoteConfigProvider.GetBattlesConfig, localConfigProvider.GetBattlesConfig, embeddedConfigProvider.LoadBattlesConfig),
                    TryLoadConfig(remoteConfigProvider.GetWarriorsConfig, localConfigProvider.GetWarriorsConfig, embeddedConfigProvider.LoadWarriorsConfig),
                    TryLoadConfig(remoteConfigProvider.GetWeaponsConfig, localConfigProvider.GetWeaponsConfig, embeddedConfigProvider.LoadWeaponsConfig),
                    TryLoadConfig(remoteConfigProvider.GetSkillsConfig, localConfigProvider.GetSkillsConfig, embeddedConfigProvider.LoadSkillsConfig)
                );

                JObject configData = configs.Item1;
                JObject dungeonsData = configs.Item2;
                JObject agesData = configs.Item3;
                JObject battlesData = configs.Item4;
                JObject warriorsData = configs.Item5;
                JObject weaponsData = configs.Item6;
                JObject skillsData = configs.Item7;


                if (configData != null)
                {
                    var task1 = UniTask.Run(() => extraConfigMapper.ExtractAdsConfig(configData));
                    var task2 = UniTask.Run(() => extraConfigMapper.ExtractShopConfig(configData));
                    var task3 = UniTask.Run(() => skillsConfigMapper.ExtractSkillConfigs(configData, localConfigContainer.GeneralSkillsConfig.SkillConfigs));
                    var task4 = UniTask.Run(() => skillsConfigMapper.ExtractAndMergeSkillsExtraConfig(configData, localConfigContainer.GeneralSkillsConfig.SkillsExtraConfig));
                    var task5 = UniTask.Run(() => extraConfigMapper.ExtractAndMergeSummoning(configData, localConfigContainer.SummoningConfigs));
                    var task6 = UniTask.Run(() => extraConfigMapper.ExtractBattleSpeedConfigs(configData));
                    var task7 = UniTask.Run(() => extraConfigMapper.ExtractFoodBoostConfig(configData));
                    var task8 = UniTask.Run(() => extraConfigMapper.ExtractCardsConfig(configData, localConfigContainer.CardsConfig));
                    var task9 = UniTask.Run(() => extraConfigMapper.ExtractGeneralDailyTaskConfig(configData));
                    var task10 = UniTask.Run(() => extraConfigMapper.ExtractFeatureSettings(configData));

                    var results = await UniTask.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10);

                    config.AdsConfig = results.Item1;
                    config.ShopConfig = results.Item2;
                    config.SkillConfigs = results.Item3;
                    config.SkillExtraConfig = results.Item4;
                    config.SummoningData = results.Item5;
                    config.BattleSpeedConfigs = results.Item6;
                    config.FoodBoostConfig = results.Item7;
                    config.CardConfigsById = results.Item8.Item2;
                    config.CardConfigsByType = results.Item8.Item1;
                    config.GeneralDailyTaskConfig = results.Item9;
                    config.FeatureSettings = results.Item10;

                    config.IconConfig = localConfigContainer.IconConfig;
                    config.DifficultyConfig = localConfigContainer.DifficultyConfig;
                    config.CardPricingConfig = localConfigContainer.CardsPricingConfig;

                    localConfigProvider.SaveConfig(configData.ToString());
                }
                else
                {
                    throw new Exception("Failed to load GameConfig from all sources.");
                }

                onProgress?.Invoke(0.5f);

                var task12 = UniTask.Run(() => dungeonConfigMapper.LoadAndMapDungeons(dungeonsData, localConfigContainer.DungeonsConfig.Dungeons));
                var task13 = UniTask.Run(() => agesConfigMapper.LoadAndMergeAgeConfigs(agesData, localConfigContainer.GeneralAgesConfig));
                var task14 = UniTask.Run(() => battlesConfigMapper.LoadAndMergeBattleConfigs(battlesData, localConfigContainer.GeneralBattleConfig));
                var task15 = UniTask.Run(() => warriorsConfigMapper.LoadAndMergeWarriorConfigs(warriorsData, localConfigContainer.GeneralWarriorsConfig));
                var task16 = UniTask.Run(() => weaponsConfigMapper.LoadAndMergeWeaponConfigs(weaponsData, localConfigContainer.GeneralWeaponConfig));
                var task17 = UniTask.Run(() => skillsConfigMapper.LoadAndMergeSkillConfigs(skillsData, localConfigContainer.GeneralSkillsConfig.SkillConfigs));

                var finalResults = await UniTask.WhenAll(task12, task13, task14, task15, task16, task17);

                config.Dungeons = finalResults.Item1;
                config.AgeConfigs = finalResults.Item2;
                config.BattleConfigs = finalResults.Item3;
                config.WarriorConfigs = finalResults.Item4;
                config.WeaponConfigs = finalResults.Item5;
                config.SkillConfigs = finalResults.Item6;

                localConfigProvider.SaveAgesConfig(agesData.ToString());
                localConfigProvider.SaveDungeonConfig(dungeonsData.ToString());
                localConfigProvider.SaveBattlesConfig(battlesData.ToString());
                localConfigProvider.SaveWarriorsConfig(warriorsData.ToString());
                localConfigProvider.SaveWeaponsConfig(weaponsData.ToString());
                localConfigProvider.SaveSkillsConfig(skillsData.ToString());


                bool isShowForcedUpdate = await remoteConfigProvider.GetIsShowForcedUpdate();

                if (isShowForcedUpdate)
                {
                    _forcedUpdateService.Initialize(isShowForcedUpdate);
                }
                else
                {
                    _logger.LogWarning("StoreVersion is not available.");
                }

                onProgress?.Invoke(1.0f);

                return config;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to load configuration: {e}");
                throw;
            }
        }

        private async UniTask<JObject> TryLoadConfig(
                Func<UniTask<JObject>> remoteLoader,
                Func<string> localLoader,
                Func<UniTask<string>> embeddedLoader)
        {
            try
            {
                // 🚀 Проверяем интернет до всякой логики
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    _logger.LogWarning("No internet. Falling back to embedded config immediately.");
                    string embeddedConfig = await embeddedLoader();
                    return JObject.Parse(embeddedConfig);
                }

                JObject config = null;

                switch (GameModeSettings.I.ConfigSource)
                {
                    case ConfigSource.Remote:
                        _logger.Log("ConfigSource REMOTE", DebugStatus.Warning);
                        config = await remoteLoader();
                        if (config == null || !IsValidConfig(config))
                        {
                            _logger.LogWarning("Remote config is null or invalid. Falling back to local config.");
                            return await LoadFallbackConfig(localLoader, embeddedLoader);
                        }

                        return config;

                    case ConfigSource.Local:
                        _logger.Log("ConfigSource LOCAL", DebugStatus.Warning);
                        string localConfig = localLoader();
                        config = JObject.Parse(localConfig);
                        if (config == null || !IsValidConfig(config))
                        {
                            _logger.LogWarning("Local config is null or invalid. Falling back to embedded config.");
                            return JObject.Parse(await embeddedLoader());
                        }

                        return config;

                    case ConfigSource.Embedded:
                        _logger.Log("ConfigSource EMBEDDED", DebugStatus.Warning);
                        string embeddedConfig = await embeddedLoader();
                        return JObject.Parse(embeddedConfig);

                    default:
                        _logger.Log("ConfigSource not specified. Using Embedded as default.", DebugStatus.Warning);
                        string defaultEmbeddedConfig = await embeddedLoader();
                        return JObject.Parse(defaultEmbeddedConfig);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Primary config load failed: {e.Message}");
                return await LoadFallbackConfig(localLoader, embeddedLoader);
            }
        }

        private async UniTask<JObject> LoadFallbackConfig(Func<string> localLoader,
            Func<UniTask<string>> embeddedLoader)
        {
            try
            {
                _logger.Log("Loading local config as fallback.", DebugStatus.Warning);
                string localConfig = localLoader();
                return JObject.Parse(localConfig);
            }
            catch (Exception localException)
            {
                _logger.LogWarning($"Failed to load local config: {localException.Message}");
                try
                {
                    _logger.Log("Loading embedded config as fallback.", DebugStatus.Warning);
                    string embeddedConfig = await embeddedLoader();
                    return JObject.Parse(embeddedConfig);
                }
                catch (Exception embeddedException)
                {
                    _logger.LogError($"Failed to load embedded config: {embeddedException.Message}");
                    return null;
                }
            }
        }

        private bool IsValidConfig(JObject config)
        {
            return config != null && config.HasValues;
        }
    }
}