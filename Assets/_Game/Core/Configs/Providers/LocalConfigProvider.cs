using _Game.Utils._LocalConfigSaver;

namespace _Game.Core.Configs.Providers
{
    public class LocalConfigProvider : ILocalConfigProvider
    {
        private const string EXTRA_CONFIG_FILE_NAME = "extraConfig.json";
        private const string DUNGEON_CONFIG_FILE_NAME = "dungeonConfig.json";
        private const string AGES_CONFIG_FILE_NAME = "agesConfig.json";
        private const string BATTLES_CONFIG_FILE_NAME = "battlesConfig.json";
        private const string WARRIORS_CONFIG_FILE_NAME = "warriorsConfig.json";
        private const string WEAPONS_CONFIG_FILE_NAME = "weaponsConfig.json";
        private const string SKILLS_CONFIG_FILE_NAME = "skillsConfig.json";
        
        public string GetConfig() => LocalConfigSaver.GetConfig(EXTRA_CONFIG_FILE_NAME);
        public string GetDungeonConfig() => LocalConfigSaver.GetConfig(DUNGEON_CONFIG_FILE_NAME);
        public string GetAgesConfig() => LocalConfigSaver.GetConfig(AGES_CONFIG_FILE_NAME);
        public string GetBattlesConfig() => LocalConfigSaver.GetConfig(BATTLES_CONFIG_FILE_NAME);
        public string GetWarriorsConfig() => LocalConfigSaver.GetConfig(WARRIORS_CONFIG_FILE_NAME);
        public string GetWeaponsConfig() => LocalConfigSaver.GetConfig(WEAPONS_CONFIG_FILE_NAME);
        public string GetSkillsConfig() => LocalConfigSaver.GetConfig(SKILLS_CONFIG_FILE_NAME);
        
        public void SaveSkillsConfig(string  config) => LocalConfigSaver.SaveConfig(config, SKILLS_CONFIG_FILE_NAME);
        public void SaveConfig(string config) => LocalConfigSaver.SaveConfig(config, EXTRA_CONFIG_FILE_NAME);
        public void SaveDungeonConfig(string config) => LocalConfigSaver.SaveConfig(config, DUNGEON_CONFIG_FILE_NAME);
        public void SaveAgesConfig(string config) => LocalConfigSaver.SaveConfig(config, AGES_CONFIG_FILE_NAME);
        public void SaveBattlesConfig(string config) => LocalConfigSaver.SaveConfig(config, BATTLES_CONFIG_FILE_NAME);

        public void SaveWarriorsConfig(string config) => LocalConfigSaver.SaveConfig(config, WARRIORS_CONFIG_FILE_NAME);
        public void SaveWeaponsConfig(string config) => LocalConfigSaver.SaveConfig(config, WEAPONS_CONFIG_FILE_NAME);
    }

    public interface ILocalConfigProvider
    {
        string GetConfig();
        string GetDungeonConfig();
        void SaveConfig(string config);
        void SaveDungeonConfig(string config);
        void SaveAgesConfig(string toString);
        string GetAgesConfig();
        string GetBattlesConfig();
        string GetWarriorsConfig();
        string GetWeaponsConfig();
        void SaveWeaponsConfig(string toString);
        void SaveWarriorsConfig(string toString);
        void SaveBattlesConfig(string toString);
        string GetSkillsConfig();
        void SaveSkillsConfig(string toString);
    }
}