using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scenes.Tests.UITest
{
    public class UITest : MonoBehaviour
    {
        [SerializeField] private RectTransform _pin;
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _rootCanvas;
        [SerializeField] private RectTransform panel;

        [SerializeField] private GameObject _gO_1;
        [SerializeField] private GameObject _gO_2;

        // private void Start()
        // {
        //     _gO_1.SetActive(false);
        //     _gO_2.SetActive(false);
        // }

        [Button]
        public void Do()
        {
            Vector3 worldPosition = _target.TransformPoint(_target.rect.center);
            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(_target.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvas, worldPosition, null, out var canvasPosition);
            _pin.anchoredPosition = canvasPosition;
        }
    }
}
