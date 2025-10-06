using _Game.Gameplay._Boosts.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class QuickBoostInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private TMP_Text _infoLabel;

        [SerializeField] private BoostType _boostType;

        public BoostType BoostType => _boostType;

        public void SetIcon(Sprite icon) => 
            _iconPlaceholder.sprite = icon;

        public void SetValue(string valueText) => 
            _infoLabel.text = valueText;
    }
}