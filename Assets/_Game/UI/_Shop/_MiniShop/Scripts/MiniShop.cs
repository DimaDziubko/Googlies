using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShop : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MiniItemShopContainer _container;
        [SerializeField] private Button[] _exitButtons;
        [SerializeField] private TMP_Text _coinsLabel;

        [SerializeField] private PopupAppearanceAnimation _animation;

        private IMiniShopPresenter _miniShopPresenter;

        private UniTaskCompletionSource<bool> _taskCompletion;
        private IAudioService _audioService;
        private CurrencyBank _bank;

        private CurrencyCell Cell => _bank.GetCell(CurrencyType.Coins);

        public MiniItemShopContainer Container => _container;

        private float _deltaPrice;

        public void Construct(
            Camera uICameraOverlay,
            IAudioService audioService,
            IUIFactory uiFactory,
            IMiniShopPresenter miniShopPresenter,
            IMyLogger logger,
            CurrencyBank bank)
        {
            _canvas.worldCamera = uICameraOverlay;
            _miniShopPresenter = miniShopPresenter;
            _audioService = audioService;
            _bank = bank;

            _miniShopPresenter.MiniShop = this;
            _container.Construct(uiFactory, logger);
        }

        public async UniTask<bool> ShowAndAwaitForDecision(float price)
        {
            _deltaPrice = price;

            _coinsLabel.text = (price - (float)Cell.Amount).ToCompactFormat(999);
            UpdateCoinsLabelColor();

            Unsubscribe();
            Subscribe();
            _miniShopPresenter.OnMiniShopOpened();

            _animation.PlayShow(OnShowComplete);

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void Subscribe()
        {
            Cell.OnAmountAdded += OnCoinsAdded;

            foreach (var button in _exitButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
        }

        private void Unsubscribe()
        {
            Cell.OnAmountAdded -= OnCoinsAdded;

            foreach (var button in _exitButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        public void ForceHide()
        {
            _miniShopPresenter.OnMiniShopClosed();
            _taskCompletion?.TrySetResult(true);
        }

        private void OnCancelled()
        {
            DisableButtons();

            _miniShopPresenter.OnMiniShopClosed();
            _audioService.PlayButtonSound();

            _animation.PlayHide(OnHideComplete);
        }

        private void DisableButtons()
        {
            foreach (var button in _exitButtons)
            {
                button.interactable = false;
            }
        }

        private void OnHideComplete()
        {
            _canvas.enabled = false;
            Cleanup();
            _taskCompletion.TrySetResult(true);
        }

        private void OnCoinsAdded(double amount) =>
            UpdateCoinsLabelColor();

        private void UpdateCoinsLabelColor() =>
            _coinsLabel.color = Cell.Amount < _deltaPrice ? Color.red : Color.white;

        private void Cleanup()
        {
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }
    }
}