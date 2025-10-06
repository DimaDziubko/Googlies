using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlotView : MonoBehaviour
    {
        public event UnityAction SlotClicked
        {
            add => _slotBtn.onClick.AddListener(value);
            remove => _slotBtn.onClick.RemoveListener(value);
        }

        public int Id => _id;

        [SerializeField] private int _id;
        [SerializeField, Required] private Button _slotBtn;

        [SerializeField, Required] private GameObject _lockedView;
        [SerializeField, Required] private TMP_Text _lockedInfoLabel;

        [SerializeField, Required] private GameObject _unlockedView;

        [SerializeField, Required] private Image _skillIconHolder;

        public void SetLocked(bool isLocked)
        {
            _lockedView.SetActive(isLocked);
            _unlockedView.SetActive(!isLocked);
        }

        public void SetLockedInfo(string info) => 
            _lockedInfoLabel.text = info;

        public void SetIcon(Sprite skillModelIcon) => 
            _skillIconHolder.sprite = skillModelIcon;

        public void SetIconActive(bool isIconActive) => 
            _skillIconHolder.enabled = isIconActive;
    }
}