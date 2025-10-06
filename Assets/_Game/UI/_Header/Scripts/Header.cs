using _Game.UI._Currencies;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Header.Scripts
{
    public class Header : MonoBehaviour
    {
        public event UnityAction SettingsClicked
        {
            add => _settingsButton.onClick.AddListener(value);
            remove => _settingsButton.onClick.RemoveListener(value);
        }

        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _screenNameLabel;
        [SerializeField] private CurrenciesUI _currenciesUI;
        [SerializeField] private Button _settingsButton;

        public void SetCamera(Camera camera) => _canvas.worldCamera = camera;

        public CurrenciesUI CurrenciesUI => _currenciesUI;
        public void SetInfo(string screenName) => _screenNameLabel.text = screenName;
    }
}
