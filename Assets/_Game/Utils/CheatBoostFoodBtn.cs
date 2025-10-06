using _Game.Core._GameMode;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils
{
    public class CheatBoostFoodBtn : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public void Initialize(Action callback)
        {
            if (!GameModeSettings.I.IsCheatEnabled)
            {
                this.Destroy();
                return;
            }

            _button.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Destroy()
        {
            Destroy(gameObject);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            Cleanup();
        }
    }
}