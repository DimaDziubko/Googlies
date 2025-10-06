using _Game.UI._Currencies;
using TMPro;
using UnityEngine;

namespace _Game.UI._Header.Scripts
{
    public class Header : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _screenNameLabel;
        [SerializeField] private CurrenciesUI _currenciesUI;

        public void SetCamera(Camera camera) => _canvas.worldCamera = camera;

        public CurrenciesUI CurrenciesUI => _currenciesUI;
        public void SetInfo(string screenName) => _screenNameLabel.text = screenName;
    }
}
