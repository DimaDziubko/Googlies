using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class SummoningView : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _summoningButton.onClick.AddListener(value);
            remove => _summoningButton.onClick.RemoveListener(value);
        }
        
        [SerializeField, Required] private Button _summoningButton;
        [SerializeField, Required] private Slider _summoningProgressSlider;
        [SerializeField, Required] private TMP_Text _summoningProgressLabel;
        [SerializeField, Required] private TMP_Text _summoningLevelLabel;

        public void SetProgress(float value) => 
            _summoningProgressSlider.value = value;

        public void SetProgress(string value) => 
            _summoningProgressLabel.text = value;

        public void SetLevel(string level) => 
            _summoningLevelLabel.text = level;
    }
}