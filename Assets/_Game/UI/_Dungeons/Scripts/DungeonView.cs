using _Game.UI._Shop.Scripts._AmountView;
using Sirenix.OdinInspector;
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
            add => _enterButton.onClick.AddListener(value);
            remove => _enterButton.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private GameObject _activeView;
        [SerializeField, Required] private GameObject _lockedView;
        [SerializeField, Required] private Image _mainImage;
        [SerializeField, Required] private TMP_Text _nameLabel;
        [SerializeField, Required] private TMP_Text _difficulty;

        [Space, Header("Playable Panel")]
        [SerializeField, Required] private Button _previousDifficultyBtn;
        [SerializeField, Required] private Button _nextDifficultyBtn;
        [SerializeField, Required] private AmountView _rewardAmountView;
        [SerializeField, Required] private ThemedButton _enterButton;
        [SerializeField, Required] private AmountView _costAmountView;

        internal Button PreviousDifficultyBtn => _previousDifficultyBtn;
        internal Button NextDifficultyBtn => _nextDifficultyBtn;

        internal AmountView RewardAmountView => _rewardAmountView;
        internal AmountView CostAmountView => _costAmountView;
        internal TMP_Text Difficulty => _difficulty;

        public void SetLocked(bool isLocked)
        {
            _activeView.SetActive(!isLocked);
            _lockedView.SetActive(isLocked);
        }

        public void SetInteractable(bool isInteractable) =>
            _enterButton.interactable = isInteractable;

        public void SetMainIcon(Sprite icon) =>
            _mainImage.sprite = icon;

        public void SetName(string name) =>
            _nameLabel.text = name;
    }
}