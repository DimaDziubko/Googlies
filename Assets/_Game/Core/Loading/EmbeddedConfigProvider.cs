using System.IO;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Game.Core.Loading
{
    public class EmbeddedConfigProvider
    {
        public async UniTask<string> LoadGameConfig() => await Load(Constants.StreamingAssets.EMBEDDED_EXTRA_CONFIG);
        public async UniTask<string> LoadAgesConfig() => await Load(Constants.StreamingAssets.EMBEDDED_AGES_CONFIG);

        public async UniTask<string> LoadBattlesConfig() => await Load(Constants.StreamingAssets.EMBEDDED_BATTLES_CONFIG);

        public async UniTask<string> LoadWarriorsConfig() => await Load(Constants.StreamingAssets.EMBEDDED_WARRIORS_CONFIG);

        public async UniTask<string> LoadWeaponsConfig() => await Load(Constants.StreamingAssets.EMBEDDED_WEAPONS_CONFIG);

        public async UniTask<string> LoadDungeonsConfig() => await Load(Constants.StreamingAssets.EMBEDDED_DUNGEON_CONFIG);
        
        public async UniTask<string> LoadSkillsConfig() => await Load(Constants.StreamingAssets.EMBEDDED_SKILLS_CONFIG);

        public async UniTask<string> Load(string jsonFileName)
        {
            string jsonData = string.Empty;
            
            string path = GetPlatformPath(jsonFileName);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // For Android and WebGL, use UnityWebRequest
                using (UnityWebRequest www = UnityWebRequest.Get(path))
                {
                    await www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError("Error: " + www.error);
                        return null;
                    }
                    else
                    {
                        jsonData = www.downloadHandler.text;
                    }
                }
            }
            else
            {
                // For other platforms, use direct file access
                if (File.Exists(path))
                {
                    jsonData = File.ReadAllText(path);
                }
                else
                {
                    Debug.LogError("Cannot find file!");
                    return null;
                }
            }

            return jsonData;
        }

        private string GetPlatformPath(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            if (Application.platform == RuntimePlatform.Android)
            {
                // For Android, files are inside a compressed APK
                path = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // For WebGL, return a HTTP URL
                path = Application.streamingAssetsPath + "/" + fileName;
            }
            
            // For other platforms, Application.streamingAssetsPath works directly
            return path;
        }
        
    }
}