using System;
using _Game.UI._Hud._BattleSpeedView;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud._SpeedBoostView.Scripts
{
    public class SpeedBoostBtnModel
    {
        public BtnState State;
        public string InfoText;
    }
    
    [RequireComponent(typeof(Button))]
    public class SpeedBoostBtn : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Image _changeableImage;

        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _loadingText;

        [SerializeField] private ThemedButton _button;

        public void Initialize(Action callback)
        {
            _button.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }
        
        public void UpdateBtnState(SpeedBoostBtnModel model)
        {
            switch (model.State)
            {
                case BtnState.Active:
                    HandleActiveState(model);
                    break;
                case BtnState.Inactive:
                    HandleInactiveState();
                    break;
                case BtnState.Activated:
                    HandleActivatedState();
                    break;
                case BtnState.Locked:
                    HandleLockedState();
                    break;
                case BtnState.Loading:
                    HandleLoadingState(model);
                    break;
            }
        }

        private void HandleLockedState() => gameObject.SetActive(false);

        private void HandleActivatedState() => gameObject.SetActive(false);
        private void HandleInactiveState() => gameObject.SetActive(false);

        private void HandleActiveState(SpeedBoostBtnModel model)
        {
            gameObject.SetActive(true);
            _changeableImage.sprite = _activeSprite;
            _valueLabel.text = model.InfoText;
            _panel.gameObject.SetActive(true);
            _button.interactable = true;
            _loadingText.enabled = false;
        }

        private void HandleLoadingState(SpeedBoostBtnModel model)
        {
            gameObject.SetActive(true);
            _changeableImage.sprite = _inactiveSprite;
            _panel.gameObject.SetActive(false);
            _loadingText.enabled = true;
            _button.interactable = false;
        }
    }
}