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

    public class GameModeSettings : MonoBehaviour
    {
        // Константа с путем к INI файлу
        private const string ENVIRONMENT_CONFIG_PATH = "Assets/_Game/LocalConfigs/environment.ini";

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
        [SerializeField, ReadOnly] private string _configFilePath = ENVIRONMENT_CONFIG_PATH;

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

            try
            {
                // Проверяем существование файла
                if (!System.IO.File.Exists(ENVIRONMENT_CONFIG_PATH))
                {
                    Debug.LogError($"Environment config file not found at: {ENVIRONMENT_CONFIG_PATH}");
                    _environmentConfig = CreateFallbackConfig();
                    return;
                }

                // Читаем содержимое файла
                string configContent = System.IO.File.ReadAllText(ENVIRONMENT_CONFIG_PATH);

                if (string.IsNullOrEmpty(configContent))
                {
                    Debug.LogError("Environment config file is empty!");
                    _environmentConfig = CreateFallbackConfig();
                    return;
                }

                Debug.Log($"Loading environment config from: {ENVIRONMENT_CONFIG_PATH}");
                Debug.Log($"Config content preview: {configContent.Substring(0, Mathf.Min(100, configContent.Length))}");

                // Парсим конфиг
                _environmentConfig = EnvironmentConfigParser.Parse(configContent);

                if (!_environmentConfig.IsValid())
                {
                    Debug.LogWarning("Invalid environment config. Using fallback.");
                    _environmentConfig = CreateFallbackConfig();
                }
                else
                {
                    Debug.Log($"Successfully loaded config. Active environment: {_environmentConfig.GetActiveEnvironment()}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error reading environment config: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
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
            return _environmentConfig.GetActiveEnvironment();// ?? GetFallbackEnvironmentType();
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

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//            Debug.unityLogger.logEnabled = true;
//#else
//            Debug.unityLogger.logEnabled = false;
//#endif
        }

        [Button("Reload Config")]
        private void ReloadConfig()
        {
            InitializeConfig();
        }

        [Button("Test INI File")]
        private void TestIniFile()
        {
            if (System.IO.File.Exists(ENVIRONMENT_CONFIG_PATH))
            {
                string content = System.IO.File.ReadAllText(ENVIRONMENT_CONFIG_PATH);
                Debug.Log($"INI file content:\n{content}");

                try
                {
                    var testConfig = EnvironmentConfigParser.Parse(content);
                    Debug.Log($"Parse successful! Active environment: {testConfig.GetActiveEnvironment()} (ID: {(int)testConfig.GetActiveEnvironment()})");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Parse failed: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"File not found: {ENVIRONMENT_CONFIG_PATH}");
            }
        }

        public bool IsDevelopment() => _detectedEnvironment == EnvironmentType.Development;
        public bool IsStaging() => _detectedEnvironment == EnvironmentType.Staging;
        public bool IsProduction() => _detectedEnvironment == EnvironmentType.Production;
    }
}