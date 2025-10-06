using System;
using _Game.Core.Services.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Toggle))]
    public class CustomToggle : MonoBehaviour
    {
        public event Action<bool> ValueChanged;

        private IAudioService _audioService;
    
        [SerializeField] private Toggle _toggle;
        private bool IsOn => _toggle.isOn;
    
        //Animation data
        [SerializeField] private RectTransform _toggleBtnTransform;
        [SerializeField] private Image _background;
        [SerializeField] private Color _onColor = Color.green;
        [SerializeField] private Color _offColor = Color.red;
    
        [SerializeField] private Vector2 _toggleOnPosition;
        [SerializeField] private Vector2 _toggleOffPosition;
        [SerializeField] private float _animationDuration;
        public void Initialize(bool isOn, IAudioService audioService)
        {
            _audioService = audioService;
        
            Cleanup();

            _toggle.isOn = isOn;

            SetupToggleBtnPosition();

            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void SetupToggleBtnPosition()
        {
            _toggleBtnTransform.anchoredPosition = IsOn ? _toggleOnPosition : _toggleOffPosition;
            _background.color = IsOn ? _onColor : _offColor;
        }

        private void OnValueChanged(bool arg0)
        {
            PlayTransitionAnimation();
            _audioService.PlayButtonSound();
            ValueChanged?.Invoke(arg0);
        }
    
        private void PlayTransitionAnimation()
        {
            if (IsOn)
            {
                PlayOn();
            }
            else
            {
                PlayOff();
            }
        }

        public void SetInteractable(bool isInteractable) => 
            _toggle.interactable = isInteractable;

        private void PlayOff()
        {
            _toggleBtnTransform.DOAnchorPos(_toggleOffPosition, _animationDuration);
            _background.DOColor(_offColor, _animationDuration);
        }

        private void PlayOn()
        {
            _toggleBtnTransform.DOAnchorPos(_toggleOnPosition, _animationDuration);
            _background.DOColor(_onColor, _animationDuration);
        }

        public void Cleanup() => _toggle.onValueChanged.RemoveAllListeners();
    }
}
