using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Functions;
using _Game.Core.Configs.Models._Skills;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class SkillsConfigMapper : BaseConfigMapper
    {
        public SkillsConfigMapper(IMyLogger logger) : base(logger)
        {
        }
        
        public SkillExtraConfig ExtractAndMergeSkillsExtraConfig(JObject jsonData, SkillExtraConfig localConfig)
        {
            _logger.Log("Extracting skill extra config", DebugStatus.Warning);
                
            var skillExtraToken = jsonData[Constants.ConfigKeys.SKILL_EXTRA_CONFIG];
            RemoteSkillExtraConfig remoteConfig = skillExtraToken?.ToObject<RemoteSkillExtraConfig>();

            if (remoteConfig != null)
            {
                MergeSkillsExtra(localConfig, remoteConfig);
            }
            else
            {
                _logger.Log("Remote extra skill is null", DebugStatus.Fault);
            }
            
            return localConfig;
        }

        private void MergeSkillsExtra(SkillExtraConfig local, RemoteSkillExtraConfig remote)
        {
            local.InitialDropList = remote.InitialDropList;
            local.RequiredTimeline = remote.RequiredTimeline;
        }
        
        public Dictionary<int, SkillConfig> ExtractSkillConfigs(JObject configData, List<SkillConfig> skillConfigs) => 
            skillConfigs.ToDictionary(x => x.Id, x => x);
        
        
        public Dictionary<int, SkillConfig> LoadAndMergeSkillConfigs(JObject jsonData, List<SkillConfig> skillConfigs)
        {
            _logger.Log("Extracting skill configs", DebugStatus.Warning);
            Dictionary<int, SkillConfig> skills = skillConfigs.ToDictionary(x => x.Id);
            List<RemoteSkillConfig> remoteDungeons = ExtractSkills(jsonData);
            
            MergeSkillConfigs(remoteDungeons, skills);
            return skills;
        }
        
        private void MergeSkillConfigs(List<RemoteSkillConfig> remoteDungeons, Dictionary<int, SkillConfig> localConfig)
        {
            foreach (var remote in remoteDungeons)
            {
                if (localConfig.TryGetValue(remote.Id, out var local))
                {
                    local.DropChance = remote.DropChance;
                    local.Boosts = new[]
                    {
                        new Boost()
                        {
                            Type = remote.PassiveType,
                            Linear = new LinearFunction(remote.PassiveLFC)
                        }
                    };

                    foreach (var boost in local.Boosts)
                    {
                        boost.UpdateNameBasedOnType();
                    }
                    
                    local.FBConfigID = remote.FBConfigID;
                }
            }
        }

        private List<RemoteSkillConfig> ExtractSkills(JObject jsonData)
        {
            try
            {
                var skillsToken = jsonData["Skills"];
                if (skillsToken == null)
                {
                    _logger.LogError("Skills key is missing in the JSON data.");
                    return new List<RemoteSkillConfig>();
                }
                
                var skills = skillsToken.ToObject<List<RemoteSkillConfig>>();
                _logger.Log($"Extracted {skills.Count} skills successfully.");
                return skills;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to extract skills: {ex.Message}");
                return new List<RemoteSkillConfig>();
            }
        }
    }
}