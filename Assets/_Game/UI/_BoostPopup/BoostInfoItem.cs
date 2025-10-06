using _Game.Gameplay._Boosts.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class BoostInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _valueLabel;

        [SerializeField] private BoostType _boostType;

        public BoostType BoostType => _boostType;

        public void SetIcon(Sprite icon) => 
            _iconPlaceholder.sprite = icon;

        public void SetValue(string valueText) => 
            _valueLabel.text = valueText;
        
        public void SetName(string name) => 
            _nameLabel.text = name;
    }
}