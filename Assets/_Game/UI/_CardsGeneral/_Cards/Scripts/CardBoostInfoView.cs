using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardBoostInfoView : MonoBehaviour
    {
        [SerializeField] private Image _boostIconPlaceholder;
        [SerializeField] private TMP_Text _boostNameLabel;
        [SerializeField] private TMP_Text _currentBoostValueLabel;
        [SerializeField] private TMP_Text _nextBoostValueLabel;

        public void SetIcon(Sprite icon) => _boostIconPlaceholder.sprite = icon;
        public void SetName(string name) => _boostNameLabel.text = name;
        public void SetCurrentValue(string currentValue) => _currentBoostValueLabel.text = currentValue;
        public void SetNextValue(string nextValue) => _nextBoostValueLabel.text = nextValue;
    }
}