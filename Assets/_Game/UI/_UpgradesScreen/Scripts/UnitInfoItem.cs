using _Game.UI._StatsPopup.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UnitInfoItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _weaponNameLabel;
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private StatInfoListView _statInfoListView;
        [SerializeField] private Image _weaponTypeImage;

        public StatInfoListView StatInfoListView => _statInfoListView;

        public void SetIcon(Sprite icon) => _iconPlaceholder.sprite = icon;
        public void SetWeaponNameText(string text) => _weaponNameLabel.text = text;
        public void SetWeaponTypeImage(Sprite sprite) => _weaponTypeImage.sprite = sprite;
    }
}

