using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._CoinBundles
{
    public class CoinsBundleView : ShopItemView
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private Image _minorProductIconHolder;
        [SerializeField] private TMP_Text _quantityLable;
        [SerializeField] private TransactionButton _button;

        public TransactionButton Button => _button;

        public override void Cleanup() => _button.Cleanup();

        public void SetMajorIcon(Sprite sprite) => 
            _majorProductIconHolder.sprite = sprite;

        public void SetMinorIcon(Sprite sprite) => 
            _minorProductIconHolder.sprite = sprite;

        public void SetQuantity(string quantity) => 
            _quantityLable.text = quantity;

        public void SetCurrencyIcon(Sprite sprite) => 
            _button.SetCurrencyIcon(sprite);
    }
}