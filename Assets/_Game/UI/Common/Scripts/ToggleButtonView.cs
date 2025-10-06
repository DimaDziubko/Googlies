using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    public class ToggleButtonView : MonoBehaviour
    {
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Image _changableImage;

        [SerializeField] private Image iconHolder;
        [SerializeField] private Sprite activeIcon;
        [SerializeField] private Sprite lockedIcon;
        
        [SerializeField] private TMP_Text lockedText;
        [SerializeField] private TMP_Text unlockedText;
        [SerializeField] private bool hideLockIcon;
        
        public void Highlight() => _changableImage.sprite = _activeSprite;

        public void UnHighlight() => _changableImage.sprite = _inactiveSprite;

        public void SetIcon(bool isLocked)
        {
            iconHolder.sprite = isLocked ? lockedIcon : activeIcon;
            if (hideLockIcon)
            {
                iconHolder.enabled = isLocked;
            }
        }

        public void SetText(bool isLocked)
        {
            if (lockedText != null && unlockedText != null)
            {
                lockedText.enabled = isLocked;
                unlockedText.enabled = !isLocked;
            }
        }
    }
}