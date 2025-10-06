using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Utils
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaAdjuster : MonoBehaviour
    {
        private RectTransform _panel;
        private Rect _lastSafeArea;

        void Awake()
        {
            _panel = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        [Button]
        void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != _lastSafeArea)
            {
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;

                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                _panel.anchorMin = anchorMin;
                _panel.anchorMax = anchorMax;

                _lastSafeArea = safeArea;
            }
        }
        
        private void OnRectTransformDimensionsChange()
        {
            if(_panel == null) _panel = GetComponent<RectTransform>();
            ApplySafeArea();
        }
    }
}
