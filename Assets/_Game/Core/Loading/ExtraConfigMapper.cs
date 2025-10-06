using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._AdsConfig;
using _Game.Core.Configs.Models._BattleSpeedConfig;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Models._DailyTaskConfig;
using _Game.Core.Configs.Models._FoodBoostConfig;
using _Game.Core.Configs.Models._GameConfig;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class ExtraConfigMapper
    {
        private readonly IMyLogger _logger;

        public ExtraConfigMapper(IMyLogger logger)
        {
            _logger = logger;
        }

        public List<BattleSpeedConfig> ExtractBattleSpeedConfigs(JObject jsonData)
        {
            _logger.Log("Extracting battle speed configs", DebugStatus.Warning);
            return ParseConfigList<BattleSpeedConfig>(jsonData, Constants.ConfigKeys.BATTLE_SPEEDS_CONFIGS);
        }

        public SummoningData ExtractAndMergeSummoning(JObject jsonData, SummoningConfigs localConfig)
        {
            _logger.Log("Extracting summoning configs", DebugStatus.Warning);

            int accumulated = 0;
            foreach (var summoning in localConfig.CardsSummoningConfigs)
            {
                accumulated += summoning.CardsRequiredForLevel;
                summoning.AccumulatedCardsRequiredForLevel = accumulated;
            }

            var summoningToken = jsonData[Constants.ConfigKeys.SUMMONING_CONFIGS];
            RemoteSummoningConfigs remoteConfig = summoningToken?.ToObject<RemoteSummoningConfigs>();

            MergeSummonings(localConfig, remoteConfig);

            var summoningData = new SummoningData
            {
                DropListsEnabled = localConfig.DropListsEnabled,
                InitialDropList = localConfig.InitialDropList,
                SummoningConfig = localConfig.CardsSummoningConfigs.ToDictionary(x => x.Level, x => x)
            };

            return summoningData;
        }

        private void MergeSummonings(SummoningConfigs local, RemoteSummoningConfigs remote)
        {
            local.DropListsEnabled = remote.DropListsEnabled;
            local.InitialDropList = remote.InitialDropList;

            var remoteCardSummoningsDict = remote.CardsSummoningConfigs.ToDictionary(x => x.Id);

            foreach (var cardsSummoning in local.CardsSummoningConfigs)
            {
                if (remoteCardSummoningsDict.TryGetValue(cardsSummoning.Id, out var remoteConfig))
                {
                    cardsSummoning.DropList = remoteConfig.DropList;
                }
            }
        }

        public (Dictionary<CardType, List<CardConfig>>, Dictionary<int, CardConfig>) ExtractCardsConfig(
            JObject jsonData, CardsConfig cardsConfig)
        {
            _logger.Log("Extracting card configs", DebugStatus.Warning);

            var cardsByType = new Dictionary<CardType, List<CardConfig>>();
            var cardsById = new Dictionary<int, CardConfig>();

            foreach (var card in cardsConfig.CardConfigs)
            {
                if (!cardsByType.ContainsKey(card.Type))
                {
                    cardsByType[card.Type] = new List<CardConfig>();
                }

                cardsByType[card.Type].Add(card);

                cardsById[card.Id] = card;
            }

            return (cardsByType, cardsById);
        }

        public GeneralDailyTaskConfig ExtractGeneralDailyTaskConfig(JObject jsonData)
        {
            _logger.Log("Extracting daily task configs", DebugStatus.Warning);

            var generalDailyTaskToken = jsonData[Constants.ConfigKeys.GENERAL_DAILY_TASK_CONFIG];
            if (generalDailyTaskToken == null)
            {
                _logger.LogError("GeneralDailyTaskConfig is null");
                return null;
            }

            return generalDailyTaskToken.ToObject<GeneralDailyTaskConfig>();
        }

        public FoodBoostConfig ExtractFoodBoostConfig(JObject jsonData)
        {
            _logger.Log("Extracting food boost configs", DebugStatus.Warning);

            var foodBoostToken = jsonData[Constants.ConfigKeys.FOOD_BOOST_CONFIG];
            if (foodBoostToken == null)
            {
                _logger.LogError("FoodBoostConfig is null");
                return null;
            }

            return foodBoostToken.ToObject<FoodBoostConfig>();
        }

        public ShopConfig ExtractShopConfig(JObject jsonData)
        {
            _logger.Log("Extracting shop configs", DebugStatus.Warning);
            var shopToken = jsonData[Constants.ConfigKeys.SHOP_CONFIG];
            if (shopToken == null)
            {
                _logger.LogError("ShopConfig is null");
                return null;
            }

            return shopToken.ToObject<ShopConfig>();
        }

        public FeatureSettings ExtractFeatureSettings(JObject jsonData)
        {
            _logger.Log("Extracting feature settings", DebugStatus.Warning);
            var featureSettings = jsonData[Constants.ConfigKeys.FEATURE_SETTINGS];
            if (featureSettings == null)
            {
                _logger.LogError("FeatureSettings is null");
                return null;
            }

            return featureSettings.ToObject<FeatureSettings>();
        }

        public AdsConfig ExtractAdsConfig(JObject jsonData)
        {
            _logger.Log("Extracting ads configs", DebugStatus.Warning);
            var adsConfigToken = jsonData[Constants.ConfigKeys.ADS_CONFIG];
            if (adsConfigToken == null)
            {
                _logger.LogError("Ads config is null");
                return null;
            }

            return adsConfigToken.ToObject<AdsConfig>();
        }

        private List<T> ParseConfigList<T>(JObject jsonData, string configKey) where T : class
        {
            var tokens = jsonData[configKey];
            if (tokens == null)
            {
                _logger.LogError($"{typeof(T).Name} config is null for key {configKey}");
                return null;
            }

            return tokens.Select(token => token.ToObject<T>()).ToList();
        }

        private Dictionary<int, T> ParseConfigDictionary<T>(JObject jsonData, string configKey)
            where T : class, IConfigWithId
        {
            var tokens = jsonData[configKey];
            if (tokens == null)
            {
                _logger.LogError($"{typeof(T).Name} config is null for key {configKey}");
                return null;
            }

            return tokens.Select(token => token.ToObject<T>())
                .ToDictionary(config => config.Id, config => config);
        }

        private Dictionary<int, T> ParseConfigDictionaryWithCustomConverter<T>(JObject jsonData, string configKey,
            JsonSerializerSettings serializerSettings) where T : class, IConfigWithId
        {
            var tokens = jsonData[configKey];
            if (tokens == null)
            {
                _logger.LogError($"{typeof(T).Name} config is null for key {configKey}");
                return null;
            }

            return tokens.Select(token => JsonConvert.DeserializeObject<T>(token.ToString(), serializerSettings))
                .ToDictionary(config => config.Id, config => config);
        }
    }
}