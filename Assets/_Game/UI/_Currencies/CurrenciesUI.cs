using UnityEngine;

namespace _Game.UI._Currencies
{
    public class CurrenciesUI : MonoBehaviour
    {
        [SerializeField] private CurrencyView _coinsView;
        [SerializeField] private CurrencyView _gemsView;

        public CurrencyView CoinsView => _coinsView;
        public CurrencyView GemsView => _gemsView;
    }
}