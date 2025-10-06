using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UnitUpgradeView : MonoBehaviour
    {
        public event UnityAction InfoClicked
        {
            add => _infoButton.onClick.AddListener(value);
            remove => _infoButton.onClick.RemoveListener(value);
        }

        public UnitType Type => _type;

        public TransactionButton TransactionButton => _transactionButton;
        
        [SerializeField] private UnitType _type;

        [SerializeField, Required] private Image _lockedBg;
        [SerializeField, Required] private Image _unlockedBg;
        
        [SerializeField, Required] private Image _infoIcon;
        
        [SerializeField, Required] private Image _unitIconHolder;
        [SerializeField] private Color _unlockedIconColor;
        [SerializeField] private Color _lockedIconColor;

        [SerializeField, Required] private TransactionButton _transactionButton;
        [SerializeField, Required] private TMP_Text _unitNameLabel;

        [SerializeField, Required] private Button _infoButton;

        [SerializeField, Required] private StatInfoListView  _statInfoListViews;

        public StatInfoListView StatInfoListViews => _statInfoListViews;

        public void SetLocked(bool isLocked)
        {
            _lockedBg.enabled = isLocked;
            _unlockedBg.enabled = !isLocked;
            
            _unitIconHolder.color = isLocked ? _lockedIconColor : _unlockedIconColor;
            _unitNameLabel.enabled = !isLocked;
            _infoButton.interactable = !isLocked;
            
            _transactionButton.SetActive(isLocked);
            
            _statInfoListViews.gameObject.SetActive(!isLocked);
            
            _infoIcon.enabled = !isLocked;
        }
        
        public void SetName(string name) => 
            _unitNameLabel.text = name;

        public void SetIcon(Sprite icon) => 
            _unitIconHolder.sprite = icon;

        public void SetCurrencyIcon(Sprite icon) => 
            _transactionButton.SetCurrencyIcon(icon);
    }
}