using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtils;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(ThemedButton))]
    public class TransactionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }
        
        public event Action HoldClicked;
        public event Action InactiveClicked;

        [SerializeField] private TMP_Text _priceLabel;
        [SerializeField] private Image _currencyIconHolder;
        [SerializeField] private ThemedButton _button;
        [SerializeField] private GameObject _moneyPanel;

        [SerializeField] private bool _isHoldable = false;

         private readonly Color _affordableColor = new(1f, 1f, 1f);

        private readonly Color _expensiveColor = new(1f, 0.3f, 0f);
        
        private bool _isPointerDown;
        private float _initialDelay = 0.5f;
        private float _repeatRate = 0.05f;

        private CancellationTokenSource _cancellationTokenSource;

        public void SetPrice(string price)
        {
            _priceLabel.text = price;
        }

        public void SetInteractable(bool isInteractable)
        {
            ThemedButton buttonObject = _button.OrNull();
            if (buttonObject == null) return;
            
            if (buttonObject.interactable && !isInteractable && _isPointerDown)
            {
                InactiveClicked?.Invoke();
            }
            
            buttonObject.SetInteractable(isInteractable);
            _priceLabel.color = isInteractable ? _affordableColor : _expensiveColor;
            
            if(!isInteractable) _isPointerDown = false; 
            
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetCurrencyIcon(Sprite currencyIcon) => 
            _currencyIconHolder.sprite = currencyIcon;

        public void HideCurrencyIcon() => 
            _currencyIconHolder.gameObject.SetActive(false);

        public void ShowCurrencyIcon() => 
            _currencyIconHolder.gameObject.SetActive(true);

        public void SetMoneyPanelActive(bool isActive) => 
            _moneyPanel.SetActive(isActive);

        public void Cleanup()
        {
            _isPointerDown = false;
            CancelAndDisposeCancellationToken();
        }

        private async UniTaskVoid ProcessHoldAction(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay), cancellationToken: cancellationToken);

                while (_isPointerDown && !cancellationToken.IsCancellationRequested)
                {
                    HoldClicked?.Invoke();
                    await UniTask.Delay(TimeSpan.FromSeconds(_repeatRate), cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (_button == null)
                return;

            if (!_button.interactable)
            {
                InactiveClicked?.Invoke();
                return;
            }
            
            if (_cancellationTokenSource != null)
            {
                CancelAndDisposeCancellationToken();
            }

            if (!_isHoldable) return;

            _isPointerDown = true;
            _cancellationTokenSource = new CancellationTokenSource();
            ProcessHoldAction(_cancellationTokenSource.Token).Forget();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
            CancelAndDisposeCancellationToken();
        }

        private void CancelAndDisposeCancellationToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}