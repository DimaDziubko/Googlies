using _Game.UI._Shop.Scripts._AmountView;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonView : MonoBehaviour
    {
        public event UnityAction Clicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private GameObject _activeView;
        [SerializeField] private Image _rewardIconHolder;
        [SerializeField] private Image _mainImage;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private ThemedButton _button;
        [SerializeField] private AmountListView _amountListView;

        [SerializeField] private GameObject _lockedView;
        [SerializeField] private TMP_Text _timelineLabel;

        public AmountListView AmountListView => _amountListView;
        
        public void SetLocked(bool isLocked)
        {
            _activeView.SetActive(!isLocked);
            _lockedView.SetActive(isLocked);
        }

        public void SetInteractable(bool isInteractable) => 
            _button.interactable = isInteractable;

        public void SetRewardIcon(Sprite icon) => 
            _rewardIconHolder.sprite = icon;

        public void SetMainIcon(Sprite icon) => 
            _mainImage.sprite = icon;

        public void SetName(string name) => 
            _nameLabel.text = name;
        
        public void SetTimeline(string timelineNumber) => 
            _timelineLabel.text = timelineNumber;
    }
}