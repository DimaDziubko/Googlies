using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


namespace _Game.UI.BattlePass.Scripts
{
    [CreateAssetMenu(fileName = "SpriteUrlLogger", menuName = "Tools/Sprite URL Logger", order = 0)]
    public class SpriteUrlLogger : ScriptableObject
    {
        public Sprite targetSprite;
        public string url;

#if UNITY_EDITOR
        [Button]
        public void LogSpriteUrl()
        {
            if (targetSprite == null)
            {
                Debug.LogWarning("[SpriteUrlLogger] targetSprite не задано");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(targetSprite);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("[SpriteUrlLogger] Не знайдено шлях до asset'у");
                return;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetSprite, out string _, out long localId))
            {
                Debug.LogWarning("[SpriteUrlLogger] Не вдалося отримати fileID");
                return;
            }

            url = $"project://database/{assetPath}?fileID={localId}&guid={guid}&type=3#{targetSprite.name}";
        }
#endif
    }
}