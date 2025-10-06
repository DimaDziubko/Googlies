using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Game.Core.Communication
{
    public class ClearGameSaveFile
    {
#if UNITY_EDITOR

        private static string SaveFolder => $"{Application.persistentDataPath}/game_saves";
        private static string FileName => "userAccountState.json";
        private static string Path => $"{SaveFolder}/{FileName}";

        [MenuItem("Project Manager/Clear JSON File")]
        public static void DeleteConfigFile()
        {
            if (!Directory.Exists(SaveFolder))
            {
                Debug.Log("Directory not exist");
            }

            // Check if the file exists
            if (File.Exists(Path))
            {
                try
                {
                    // Attempt to delete the file
                    File.Delete(Path);
                    Debug.Log("Config file deleted successfully.");
                }
                catch (IOException ex)
                {
                    // Handle exceptions if any, such as when the file is in use or if there are permission issues
                    Debug.LogError("Error deleting config file: " + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning("Config file not found.");
            }
        }

        [MenuItem("Project Manager/Clear All PP Saves")]
        public static void ClearAllPPSaves()
        {
            Debug.LogWarning("Player Prefs was cleared");
            PlayerPrefs.DeleteAll();
        }
#endif
    }
}
