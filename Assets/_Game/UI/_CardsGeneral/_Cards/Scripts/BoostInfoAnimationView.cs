using _Game.UI._BoostPopup._Game.UI._BoostPopup;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    [RequireComponent(typeof(FadeAnimation))]
    public class BoostInfoAnimationView : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _deltaValueLabel;
        [SerializeField] private FadeAnimation _fadeAnimation;

        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);
        public void SetIcon(Sprite icon) => _iconPlaceholder.sprite = icon;
        public void SetValue(string value) => _valueLabel.text = value;
        public void SetDeltaValue(string deltaValue) => _deltaValueLabel.text = deltaValue;

        [Button]
        public void PlayFade() => _fadeAnimation.Play(Disable);
        public void Cleanup() => _fadeAnimation.Cleanup();
    }
}