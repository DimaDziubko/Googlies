using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class UpgradeNotifier : MonoBehaviour
    {
        [SerializeField] private Image _notifier;
        [SerializeField] private Animation _animation;
        
        public void SetActive(bool isActive)
        {
            _notifier.enabled = isActive;
            
            if (isActive) _animation.Play();
            else
            {
                _animation.Stop();
            }
        }
    }
}