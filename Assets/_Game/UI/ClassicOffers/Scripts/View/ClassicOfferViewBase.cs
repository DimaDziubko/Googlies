using _Game.Core.Services.Audio;
using _Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.ClassicOffers.Scripts
{
    public class ClassicOfferViewBase : MonoBehaviour
    {
        public event Action OnPurchase;

        [SerializeField] private Canvas _canvas;
        [SerializeField] protected Button[] _cancelButtons;
        [Space]
        [SerializeField] protected TransactionButton _purchaseButton;


        [SerializeField] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;

        public void Construct(Camera uICamera, IAudioService audioService)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;

            Unsubscribe();
            Subscribe();
        }

        public async UniTask<bool> ShowAndAwaitForExit()
        {
            _animation.PlayShow(OnShowComplete);

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        public void Close()
        {
            DisableButtons();
            _animation.PlayHide(OnHideCompleteAccept);
        }

        public void SetPurchaseButton(string price)
        {
            _purchaseButton.SetActive(true);
            _purchaseButton.SetInteractable(true);
            _purchaseButton.SetPrice(price);
            _purchaseButton.HideCurrencyIcon();
        }

        public void SetPurchaseBtnEnabled(bool state)
        {
            _purchaseButton?.SetActive(state);
        }

        protected virtual void Subscribe()
        {
            _purchaseButton.ButtonClicked += OnPurchaseVoid;

            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCloseBtnClick);
            }
        }

        protected virtual void Unsubscribe()
        {
            _purchaseButton.ButtonClicked -= OnPurchaseVoid;

            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveListener(OnCloseBtnClick);
            }
        }

        protected void PlayButtonSound() => _audioService.PlayButtonSound();

        protected void DisableButtons()
        {
            foreach (var button in _cancelButtons)
            {
                button.interactable = false;
            }
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void OnCloseBtnClick()
        {
            PlayButtonSound();
            Close();
        }

        private void OnPurchaseVoid()
        {
            PlayButtonSound();
            OnPurchase?.Invoke();
        }

        private void OnHideCompleteAccept()
        {
            Unsubscribe();
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }
    }
}
