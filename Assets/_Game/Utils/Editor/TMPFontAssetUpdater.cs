using TMPro;
using UnityEditor;
using UnityEngine;

namespace _Game.Utils.Editor
{
    public class TMPFontAssetUpdater : MonoBehaviour
    {
        [MenuItem("Tools/Update TMP Font Assets")]
        public static void UpdateTMPFontAssets()
        {
            TMP_FontAsset newFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/Baloo-Regular SDF");

            if (newFontAsset == null)
            {
                Debug.LogError("Font Assets not found");
                return;
            }

            TMP_Text[] textObjects = FindObjectsOfType<TMP_Text>();

            foreach (TMP_Text textObj in textObjects)
            {
                textObj.font = newFontAsset;
                EditorUtility.SetDirty(textObj);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>All Font Assets updated.</color>");
        }
    }
}
