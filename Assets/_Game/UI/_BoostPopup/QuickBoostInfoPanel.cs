using _Game.Core.Boosts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class QuickBoostInfoPanel : MonoBehaviour
    {
        public event UnityAction Clicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }
        
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private QuickBoostInfoItem[] _items;

        [SerializeField] private BoostSource _mainSource;
        [SerializeField] private BoostSource _subSource;
        public BoostSource MainSource => _mainSource;
        public BoostSource SubSource => _subSource;
        
        public QuickBoostInfoItem[] Items => _items;

    }
}