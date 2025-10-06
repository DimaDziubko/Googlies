using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Utils;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class AgesConfigMapper : BaseConfigMapper
    {
        public AgesConfigMapper(IMyLogger logger) : base(logger)
        {
            
        }
        
        public Dictionary<int, AgeConfig> LoadAndMergeAgeConfigs(JObject jsonData, GeneralAgesConfig generalAgesConfig)
        {
            _logger.Log("Extracting age configs", DebugStatus.Warning);

            List<RemoteAgeConfig> remoteAgesData =
                ParseConfigList<RemoteAgeConfig>(jsonData, Constants.ConfigKeys.AGES);
            
            Dictionary<int, AgeConfig> localAgeConfigs = generalAgesConfig.AgeConfigs.ToDictionary(x => x.Id);
            
            MergeAgeConfigs(remoteAgesData, localAgeConfigs);
            return localAgeConfigs;
        }
        
        private void MergeAgeConfigs(List<RemoteAgeConfig> remoteAgesData, Dictionary<int, AgeConfig> localAgeConfigs)
        {
            foreach (var remoteAge in remoteAgesData)
            {
                if (localAgeConfigs.TryGetValue(remoteAge.Id, out var localAge))
                {
                    localAge.Economy = remoteAge.Economy;
                    localAge.WarriorsId = remoteAge.WarriorsId;
                    localAge.GemsPerAge = remoteAge.GemsPerAge;
                    localAge.Price = remoteAge.Price;
                    localAge.Level = remoteAge.Level;
                    localAge.Order = remoteAge.Order;
                    localAge.FBConfigId = remoteAge.FBConfigId;
                }
            }
        }
    }
}