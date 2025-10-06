using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Core.Loading
{
    public class DungeonConfigMapper
    {
        private readonly IMyLogger _logger;

        public DungeonConfigMapper(IMyLogger logger)
        {
            _logger = logger;
        }
        
        public Dictionary<int, DungeonConfig> LoadAndMapDungeons(JObject jsonData, List<DungeonConfig> dungeonConfigs)
        {
            _logger.Log("Extracting dungeon configs", DebugStatus.Warning);
            Dictionary<int, DungeonConfig> dungeons = dungeonConfigs.ToDictionary(x => x.Id);
            List<DungeonRemoteConfig> remoteDungeons = ExtractDungeons(jsonData);
            
            MergeDungeonConfigs(remoteDungeons, dungeons);
            return dungeons;
        }
        
        private void MergeDungeonConfigs(List<DungeonRemoteConfig> remoteDungeons, Dictionary<int, DungeonConfig> localConfig)
        {
            foreach (var remote in remoteDungeons)
            {
                if (localConfig.TryGetValue(remote.Id, out var local))
                {
                    local.RequiredTimeline = remote.RequiredTimeline;
                    local.Name = remote.Name;
                    local.RecoveryTimeMinutes = remote.RecoveryTimeMinutes;

                    local.KeysCount = remote.KeysCount;
                    local.VideosCount = remote.VideosCount;

                    local.RewardFunction.SetValues(remote.RewardLFC);

                    local.LightWarriorCountFunction.SetValues(remote.LightWarriorLFC);
                    local.MediumWarriorCountFunction.SetValues(remote.MediumWarriorLFC);
                    local.HeavyWarriorCountFunction.SetValues(remote.HeavyWarriorLFC);
                    local.Difficulty.SetValues(remote.DifficultyLFC);
                    local.Difficulty.SetValues(remote.DifficultyLFC);
                    
                    local.ExpDifficulty.SetValues(remote.ExpDifficultyEFC);
                    local.IsExponentionDifficultyUsed = remote.IsExpDifficulty;
                    local.StartDifficulty = remote.StartDifficulty;

                    local.Delay = remote.Delay;
                    local.Cooldown = remote.Cooldown;
                    local.FoodProduction = remote.FoodProduction;
                    local.InitialFoodAmount = remote.InitialFoodAmount;
                    
                    local.RewardType = remote.RewardType;
                    local.IsLocked = remote.IsLocked;
                    local.FBConfigId = remote.FBConfigId;
                }
            }
        }

        private List<DungeonRemoteConfig> ExtractDungeons(JObject jsonData)
        {
            try
            {
                var dungeonsToken = jsonData["Dungeons"];
                if (dungeonsToken == null)
                {
                    _logger.LogError("Dungeons key is missing in the JSON data.");
                    return new List<DungeonRemoteConfig>();
                }
                
                var dungeons = dungeonsToken.ToObject<List<DungeonRemoteConfig>>();
                _logger.Log($"Extracted {dungeons.Count} dungeons successfully.");
                return dungeons;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to extract dungeons: {ex.Message}");
                return new List<DungeonRemoteConfig>();
            }
        }
    }
}