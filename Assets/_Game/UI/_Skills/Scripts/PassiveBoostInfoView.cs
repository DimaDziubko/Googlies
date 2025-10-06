using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class PassiveBoostInfoView : MonoBehaviour
    {
        [SerializeField, Required] private Image _boostIconPlaceholder;
        [SerializeField, Required] private GameObject _arrow;
        [SerializeField, Required] private TMP_Text _boostNameLabel;
        [SerializeField, Required] private TMP_Text _currentBoostValueLabel;
        [SerializeField, Required] private TMP_Text _nextBoostValueLabel;

        public void SetIcon(Sprite icon) => _boostIconPlaceholder.sprite = icon;
        public void SetName(string name) => _boostNameLabel.text = name;
        public void SetCurrentValue(string currentValue) => _currentBoostValueLabel.text = $"x{currentValue}";
        public void SetNextValue(string nextValue) => _nextBoostValueLabel.text = $"x{nextValue}";

        public void SetNextValueActive(bool isActive)
        {
            _arrow.SetActive(isActive);
            _nextBoostValueLabel.gameObject.SetActive(isActive);
        }
    }
}