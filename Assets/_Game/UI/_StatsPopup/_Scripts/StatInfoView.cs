using _Game.UI.UpgradesAndEvolution.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StatsPopup._Scripts
{
    public class StatInfoView : MonoBehaviour
    {
        [SerializeField] private Image _statIcon;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private StatInfoItemAnimation _animation;
        
        public void SetValue(string value) =>  _valueLabel.text = value;
        public void SetIcon(Sprite icon) => _statIcon.sprite = icon;

        public void AddStat(string shownValue, string newValue) => 
            _animation.Play(shownValue, newValue);
        
        public void Cleanup() => _animation.Cleanup();
    }
}