using _Game.Core._GameMode;
using Balancy;
using UnityEngine;

namespace _Game.Core.Configs.Build
{
    [CreateAssetMenu(fileName = "BuildConfig_", menuName = "Config/Build Config")]
    public class BuildConfig : ScriptableObject
    {
        [Header("Environment Info")]
        public EnvironmentType Environment = EnvironmentType.Development;

        [Space, Header("Debug & Cheats")]
        public bool CheatsEnabled = false;
        //public bool DebugLogsEnabled = true;
        public bool ShowFPS = false;

        [Space, Header("Config Settings")]
        public bool TestMode = false;
        public ConfigSource FirebaseConfigSource;
        public Constants.Environment BalancyConfig;


        [Space, Header("SDK Settings")]
        //public bool EnableAnalytics = true;
        public bool IsTestAds;

        public bool LogEnabled = false;
    }

    public enum EnvironmentType
    {
        Development,
        Staging,
        Production
    }
}