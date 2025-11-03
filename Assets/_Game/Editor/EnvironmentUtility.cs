#if UNITY_EDITOR
using _Game.Core._GameMode;
using _Game.Core.Configs.Build;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace _Game.Editor
{
    public class EnvironmentUtility
    {
        const string LOCAL_CONFIGS_PATH = "Assets/_Game/LocalConfigs";
        const string ENV_FILE_NAME = "environment.ini.bytes";

        [MenuItem("Tools/Environment/Create Development")]
        static void CreateDev() => CreateEnvironmentConfig(EnvironmentType.Development);

        [MenuItem("Tools/Environment/Create Staging")]
        static void CreateStaging() => CreateEnvironmentConfig(EnvironmentType.Staging);

        [MenuItem("Tools/Environment/Create Production")]
        static void CreateProduction() => CreateEnvironmentConfig(EnvironmentType.Production);

        [MenuItem("Tools/Environment/Quick Switch/→ Development")]
        static void QuickDev() => CreateEnvironmentConfig(EnvironmentType.Development);

        [MenuItem("Tools/Environment/Quick Switch/→ Staging")]
        static void QuickStaging() => CreateEnvironmentConfig(EnvironmentType.Staging);

        [MenuItem("Tools/Environment/Quick Switch/→ Production")]
        static void QuickProduction() => CreateEnvironmentConfig(EnvironmentType.Production);

        static void CreateEnvironmentConfig(EnvironmentType activeEnvironment)
        {
            EnsureLocalConfigsDirectory();

            string filePath = Path.Combine(LOCAL_CONFIGS_PATH, ENV_FILE_NAME);
            string content = GenerateIniContent(activeEnvironment);

            File.WriteAllText(filePath, content);
            AssetDatabase.Refresh();

            Debug.Log($"Environment set to: {activeEnvironment} (ID: {(int)activeEnvironment})");
            ShowFileInProject(filePath);
        }

        static string GenerateIniContent(EnvironmentType activeEnvironment)
        {
            var sb = new StringBuilder();

            sb.AppendLine("; Environment Configuration");
            sb.AppendLine($"; Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"; Active: {activeEnvironment} (ID: {(int)activeEnvironment})");
            sb.AppendLine(";");
            sb.AppendLine("; Environment IDs:");
            sb.AppendLine("; 0 = Development");
            sb.AppendLine("; 1 = Staging");
            sb.AppendLine("; 2 = Production");
            sb.AppendLine();
            sb.AppendLine("[Environment]");
            sb.AppendLine($"environment_id = {(int)activeEnvironment}");

            return sb.ToString();
        }

        [MenuItem("Tools/Environment/Show Current Config")]
        static void ShowCurrentConfig()
        {
            string filePath = Path.Combine(LOCAL_CONFIGS_PATH, ENV_FILE_NAME);

            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                Debug.Log($"Current environment config:\n{content}");
                ShowFileInProject(filePath);
            }
            else
            {
                Debug.LogWarning("No environment config found. Create one first.");
            }
        }

        [MenuItem("Tools/Environment/Validate Config")]
        static void ValidateConfig()
        {
            string filePath = Path.Combine(LOCAL_CONFIGS_PATH, ENV_FILE_NAME);

            if (!File.Exists(filePath))
            {
                Debug.LogError("Environment config not found!");
                return;
            }

            try
            {
                var config = EnvironmentConfigParser.Parse(File.ReadAllText(filePath));
                var activeEnv = config.GetActiveEnvironment();

                Debug.Log($"Configuration is valid. Active environment: {activeEnv} (ID: {(int)activeEnv})");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error validating config: {e.Message}");
            }
        }

        static void EnsureLocalConfigsDirectory()
        {
            if (!Directory.Exists(LOCAL_CONFIGS_PATH))
            {
                Directory.CreateDirectory(LOCAL_CONFIGS_PATH);

                string gitignorePath = Path.Combine(LOCAL_CONFIGS_PATH, ".gitignore");
                File.WriteAllText(gitignorePath, "# Local environment configs\n*.ini\n");
            }
        }

        static void ShowFileInProject(string filePath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }
    }
}
#endif