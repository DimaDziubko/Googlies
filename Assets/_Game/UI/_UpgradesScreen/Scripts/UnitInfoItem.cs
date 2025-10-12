using _Game.UI._StatsPopup.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UnitInfoItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timelineInfoLabel;
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private StatInfoListView _statInfoListView;

        public StatInfoListView StatInfoListView => _statInfoListView;

        public void SetIcon(Sprite icon) => _iconPlaceholder.sprite = icon;
        public void SetTimelineText(string text) => _timelineInfoLabel.text = text;
    }
}

