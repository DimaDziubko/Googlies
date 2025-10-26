using _Game.UI._Shop.Scripts.Common;
using TMPro;
using UnityEngine;

namespace _Game.UI._BoostPopup
{
    public class BoostInfoContainer : ListView<BoostInfoTextItem>
    {
        [SerializeField] private TMP_Text _boostInfoPanelNameLabel;
        [SerializeField] private BoostInfoListView _boostInfoListView;

        public BoostInfoListView BoostInfoListView => _boostInfoListView;
        
        public void SetName(string name) => 
            _boostInfoPanelNameLabel.text = name;
    }
}