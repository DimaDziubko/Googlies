using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._AdsGemsPack
{
    public class AdsGemsPackView : ShopItemView
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private Image _minorProductIconHolder;
        [SerializeField] private TMP_Text _quantityLabel;
        
        [SerializeField] private TMP_Text _timerLabel;
        [SerializeField] private GameObject _timerView;

        [SerializeField] private TMP_Text _loadingLabel;
        [SerializeField] private GameObject _loadingView;
        
        [SerializeField] private TransactionButton _button;
        
        public TransactionButton Button => _button;
        
        public override void Cleanup() => _button.Cleanup();

        public void SetMajorIcon(Sprite sprite) => 
            _majorProductIconHolder.sprite = sprite;

        public void SetMinorIcon(Sprite sprite) => 
            _minorProductIconHolder.sprite = sprite;

        public void SetQuantity(string quantity) => 
            _quantityLabel.text = quantity;

        public void SetActiveTimerView(bool isActive) => _timerView.SetActive(isActive);
        public void UpdateTimer(string remainingTime) => _timerLabel.text = remainingTime;
        public void SetActiveLoadingView(bool isActive) => _loadingView.SetActive(isActive);
        public void SetLoadingText(string info) => _loadingLabel.text = info;
    }
}