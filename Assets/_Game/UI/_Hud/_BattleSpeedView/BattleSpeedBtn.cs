using System;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud._BattleSpeedView
{
    public enum BtnState
    {
        Active,
        Inactive,
        Activated,
        Locked,
        Loading
    }

    public class BattleSpeedBtnModel
    {
        public BtnState State;
        public string InfoText;
        public float TimerTime;
    }

    [RequireComponent(typeof(Button))]
    public class BattleSpeedBtn : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _timerText;

        [SerializeField] private ThemedButton _button;

        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private float _normalSizeX = 155f;
        [SerializeField] private float _normalSizeY = 60f;
        [SerializeField] private float _activatedSizeX = 155f;
        [SerializeField] private float _activatedSizeY = 90f;
        [SerializeField] private float _timerColorTreshold = 5f;

        public void Initialize(Action callback)
        {
            _button.onClick.AddListener(() =>
            {
                callback?.Invoke();
            });
        }

        public void UpdateBtnState(BattleSpeedBtnModel model)
        {
            switch (model.State)
            {
                case BtnState.Locked:
                    HandleLockedState(model);
                    break;
                case BtnState.Active:
                    HandleActiveState(model);
                    break;
                case BtnState.Inactive:
                    HandleInactiveState(model);
                    break;
                case BtnState.Activated:
                    HandleActivatedState(model);
                    break;
            }
        }

        private void HandleLockedState(BattleSpeedBtnModel model) => 
            gameObject.SetActive(false);

        private void HandleInactiveState(BattleSpeedBtnModel model) => 
            gameObject.SetActive(false);


        private void HandleActiveState(BattleSpeedBtnModel model)
        {
            gameObject.SetActive(true);
            _infoText.text = model.InfoText;
            _timerText.enabled = false;
            _button.interactable = true;
            _buttonTransform.sizeDelta = new Vector2(_normalSizeX, _normalSizeY);
        }
        
        
        private void HandleActivatedState(BattleSpeedBtnModel model)
        {
            gameObject.SetActive(true);
            _infoText.text = model.InfoText;
            _timerText.enabled = true;
            UpdateTimer(model.TimerTime);
            _button.interactable = true;
            _buttonTransform.sizeDelta = new Vector2(_activatedSizeX, _activatedSizeY);
        }
        
        public void UpdateTimer(float timeLeft)
        {
            SetColor(timeLeft);
            
            //TODO: Fix here
            //_timerText.text = timeLeft.;
        }

        private void SetColor(float timeLeft)
        {
            Color colorToSet = Color.green;

            if (timeLeft < _timerColorTreshold)
                colorToSet = Color.red;

            if (_timerText.color != colorToSet)
                _timerText.color = colorToSet;
        }
    }
}