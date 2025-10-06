using _Game.Core.Configs.Repositories;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class FbConfigTracker
    {
        private readonly IAnalyticsEventSender _sender;
        private readonly IConfigRepository _config;

        public FbConfigTracker(
            IAnalyticsEventSender sender, 
            IConfigRepository config)
        {
            _sender = sender;
            _config = config;
        }
        public void Initialize()
        {
            FBConfigIDTO dto = _config.GetFBConfigIds();
            
            var parameters = new DTDCustomEventParameters();
            parameters.Add("AgesID", dto.AgesID);
            parameters.Add("BattlesID", dto.BattlesID);
            parameters.Add("DungeonsID", dto.DungeonsID);
            parameters.Add("ExtraConfigID", dto.ExtraConfigID);
            parameters.Add("WarriorsID", dto.WarriorsID);
            parameters.Add("WeaponsID", dto.WeaponsID);
            parameters.Add("SkillsID", dto.SkillsID);

            _sender.CustomEvent("firebase_config_received", parameters);
        }

        public void Dispose()
        {
            
        }
    }
    
    public class FBConfigIDTO
    {
        public string AgesID;
        public string BattlesID;
        public string DungeonsID;
        public string ExtraConfigID;
        public string WarriorsID;
        public string WeaponsID;
        public string SkillsID;
    }
}