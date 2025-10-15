using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils.Extensions
{
    public static class ScrollViewExtensions
    {
        public static void SnapScrollToTarget(this ScrollRect scrollRect, RectTransform target)
        {
            if (scrollRect == null || target == null)
            {
                Debug.LogError($"❌ NULL: scrollRect={scrollRect}, target={target}");
                return;
            }

            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            if (content == null || viewport == null)
            {
                Debug.LogError($"❌ NULL: content={content}, viewport={viewport}");
                return;
            }

            Debug.Log($"📐 Content size: {content.rect.size}, Viewport size: {viewport.rect.size}");
            Debug.Log($"📍 Target position: {target.position}, Target local: {content.InverseTransformPoint(target.position)}");

            Vector2 targetLocalPos = (Vector2)content.InverseTransformPoint(target.position);
            float contentWidth = content.rect.width;
            float viewportWidth = viewport.rect.width;

            Debug.Log($"📏 Content width: {contentWidth}, Viewport width: {viewportWidth}");

            if (Mathf.Approximately(contentWidth, viewportWidth))
            {
                Debug.LogWarning("⚠️ Content width equals viewport width - no scrolling needed");
                return;
            }

            float targetCenterX = targetLocalPos.x - target.rect.width / 2f;
            float scrollPos = (targetCenterX - viewportWidth / 2f) / (contentWidth - viewportWidth);

            Debug.Log($"🎯 targetCenterX: {targetCenterX}, scrollPos: {scrollPos}");

            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollPos);
            Debug.Log($"🔄 Final normalized pos: {scrollRect.horizontalNormalizedPosition}");
        }
    }
}
