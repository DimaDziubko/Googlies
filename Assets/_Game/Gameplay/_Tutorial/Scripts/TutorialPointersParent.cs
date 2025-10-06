using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialPointersParent : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _rectTransform;
        [SerializeField, Required] private CanvasScaler _canvasScaler;

        public RectTransform RectTransform => _rectTransform;
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}