using _Game.UI.Factory;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SkillItemView : MonoBehaviour
    {
        public event UnityAction ActiveSkillClicked
        {
            add => _skillBtn.onClick.AddListener(value);
            remove => _skillBtn.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Button _skillBtn;

        [SerializeField, Required] private Slider _progressBar;
        [SerializeField, Required] private TMP_Text _progressLabel;
        [SerializeField, Required] private TMP_Text _levelLabel;
        
        [SerializeField, Required] private UpgradeNotifier _upgradeNitifier;

        [SerializeField, Required] private SkillView _skillView;
        [SerializeField, Required] private Image _barFillImage;
        
        [SerializeField, Required] private Image _iconEquipMask;
        [SerializeField, Required] private TMP_Text _equippedText;
        
        [SerializeField] private Image[] _stars;
        
        [SerializeField, Required] private ImageColorBinder _barColorBinder;
        
        public SkillView SkillView => _skillView;
        public Transform Transform => _transform;
        public IUIFactory OriginFactory { get; set; }

        public void SetProgressValue(float value)
        {
            _progressBar.value = value;
        }

        public void SetProgress(string progress)
        {
            _progressLabel.text = progress;
        }
        
        public void Release()
        {
            OriginFactory.Reclaim(this);
        }

        public void SetEquipped(bool equipped)
        {
            _iconEquipMask.enabled = equipped;
            _equippedText.enabled = equipped;
        }
        
        public void EnableStars(int count)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].enabled = i < count;
            }
        }
        
        public void SetActiveUpgradeNotifier(bool isReady) => 
            _upgradeNitifier.SetActive(isReady);

        public void SetBarColor(Color color) => 
            _barFillImage.color = color;
        
        public void SetBarColor(string color) => 
            _barColorBinder.SetColorByName(color);

        public void SetLevel(string level) => 
            _levelLabel.text = level;
    }
}