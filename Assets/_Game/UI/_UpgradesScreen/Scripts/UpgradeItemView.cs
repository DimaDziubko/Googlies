using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private Image _iconHolder;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TransactionButton _transactionButton;
        
        public TransactionButton Button => _transactionButton;
        public void SetIcon(Sprite icon) => _iconHolder.sprite = icon;
        public void SetUpgradeName(string name) => _nameLabel.text = name;
        public void SetValue(string value) => _valueLabel.text = value;
        public void SetCurrencyIcon(Sprite icon) => _transactionButton.SetCurrencyIcon(icon);
    }
}