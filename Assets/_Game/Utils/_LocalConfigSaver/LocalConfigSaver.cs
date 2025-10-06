using System;
using System.IO;
using UnityEngine;

namespace _Game.Utils._LocalConfigSaver
{
    public static class LocalConfigSaver
    {
         private static readonly string ConfigFolderPath = Path.Combine(Application.persistentDataPath, "Config");
        
        public static void SaveConfig(string config, string fileName = "extraConfig.json")
        {
            try
            {
                if (!Directory.Exists(ConfigFolderPath))
                {
                    Directory.CreateDirectory(ConfigFolderPath);
                }

                string filePath = GetFilePath(fileName);
                File.WriteAllText(filePath, config);
                Debug.Log($"Configuration saved locally: {fileName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving configuration: {e.Message}");
            }
        }
        
        public static string GetConfig(string fileName = "gameConfig.json")
        {
            try
            {
                string filePath = GetFilePath(fileName);

                if (File.Exists(filePath))
                {
                    string configString = File.ReadAllText(filePath);
                    Debug.Log($"Configuration loaded locally: {fileName}");
                    return configString;
                }
                else
                {
                    Debug.LogWarning($"Local configuration file not found: {fileName}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading local configuration: {e.Message}");
                return null;
            }
        }

        public static void DeleteConfig(string fileName = "extraConfig.json")
        {
            try
            {
                string filePath = GetFilePath(fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"Configuration deleted locally: {fileName}");
                }
                else
                {
                    Debug.LogWarning($"No configuration file to delete: {fileName}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting configuration: {e.Message}");
            }
        }
        
        private static string GetFilePath(string fileName)
        {
            return Path.Combine(ConfigFolderPath, fileName);
        }
    }
}