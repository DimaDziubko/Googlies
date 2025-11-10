using _Game.Core.Configs.Build;
using Balancy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core._GameMode
{
    public enum ConfigSource
    {
        Remote,
        Local,
        Embedded
    }

    /// <summary>
    /// Game Mode Settings Manager. Manages environment and build configurations. Configs via scriptable objects and INI file.
    /// </summary>
    public class GameModeSettings : MonoBehaviour
    {
        [SerializeField] private TextAsset _environmentIniFile;
        [Space]
        [Header("Config Assets")]
        [SerializeField] private BuildConfig devConfig;
        [SerializeField] private BuildConfig stageConfig;
        [SerializeField] private BuildConfig prodConfig;

        [Header("Environment Settings")]
        [Space]
        [SerializeField] private bool useEnvironmentOverride = false;
        [SerializeField] private EnvironmentType environmentOverride = EnvironmentType.Development;

        [Header("Runtime Info")]
        [SerializeField, ReadOnly] private EnvironmentType _detectedEnvironment;
        [SerializeField, ReadOnly] private EnvironmentConfig _environmentConfig;

        private BuildConfig _currentConfig;

        public static GameModeSettings I { get; private set; }

        public BuildConfig CurrentConfig => _currentConfig;
        public EnvironmentType DetectedEnvironment => _detectedEnvironment;
        public EnvironmentConfig EnvironmentConfig => _environmentConfig;

        public bool TestMode => _currentConfig?.TestMode ?? false;
        public ConfigSource ConfigSource => _currentConfig?.FirebaseConfigSource ?? ConfigSource.Embedded;
        public Constants.Environment BalancyConfig => _currentConfig?.BalancyConfig ?? Constants.Environment.Production;
        public bool IsTestAds => _currentConfig?.IsTestAds ?? false;
        public bool IsCheatEnabled => _currentConfig?.CheatsEnabled ?? false;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
                DontDestroyOnLoad(gameObject);
                InitializeConfig();
            }
            else
            {
                Destroy(gameObject);
            }

            Application.targetFrameRate = 60;
        }

        private void InitializeConfig()
        {
            LoadEnvironmentConfiguration();
            _detectedEnvironment = DetermineActiveEnvironment();

            _currentConfig = _detectedEnvironment switch
            {
                EnvironmentType.Development => devConfig,
                EnvironmentType.Staging => stageConfig,
                EnvironmentType.Production => prodConfig,
                _ => devConfig
            };

            if (_currentConfig == null)
            {
                Debug.LogError($"Config for {_detectedEnvironment} environment not assigned! Using dev config.");
                _currentConfig = devConfig;
            }

            Debug.Log($"Environment: {_detectedEnvironment} | Config: {_currentConfig?.name ?? "NULL"}");
            ApplyConfig();
        }

        private void LoadEnvironmentConfiguration()
        {
            if (useEnvironmentOverride)
            {
                Debug.Log($"Using environment override: {environmentOverride}");
                _environmentConfig = CreateConfigForEnvironment(environmentOverride);
                return;
            }
            string configContent = _environmentIniFile.text;

            if (string.IsNullOrEmpty(configContent))
            {
                Debug.LogError("Environment config is empty!");
                _environmentConfig = CreateFallbackConfig();
                return;
            }

            Debug.Log($"Loaded environment.ini from Resources, length = {configContent.Length}");

            _environmentConfig = EnvironmentConfigParser.Parse(configContent);

            if (!_environmentConfig.IsValid())
            {
                Debug.LogWarning("Invalid environment config. Using fallback.");
                _environmentConfig = CreateFallbackConfig();
            }
        }


        private EnvironmentConfig CreateConfigForEnvironment(EnvironmentType envType)
        {
            return new EnvironmentConfig
            {
                ActiveEnvironment = envType
            };
        }

        private EnvironmentConfig CreateFallbackConfig()
        {
            var fallbackEnv = GetFallbackEnvironmentType();
            return CreateConfigForEnvironment(fallbackEnv);
        }

        private EnvironmentType DetermineActiveEnvironment()
        {
            return _environmentConfig.GetActiveEnvironment();
        }

        private EnvironmentType GetFallbackEnvironmentType()
        {
#if UNITY_EDITOR
            return EnvironmentType.Development;
#elif DEVELOPMENT_BUILD
            return EnvironmentType.Staging;
#else
            return EnvironmentType.Production;
#endif
        }

        private void ApplyConfig()
        {
            if (_currentConfig == null) return;

            Debug.unityLogger.logEnabled = _currentConfig.LogEnabled;

        }

        [Button("Reload Config")]
        private void ReloadConfig()
        {
            InitializeConfig();
        }

        public bool IsDevelopment() => _detectedEnvironment == EnvironmentType.Development;
        public bool IsStaging() => _detectedEnvironment == EnvironmentType.Staging;
        public bool IsProduction() => _detectedEnvironment == EnvironmentType.Production;
    }
}