using System.Globalization;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.UI.BattlePass.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPurchasePopup : MonoBehaviour
    {
        [SerializeField, Required] private UIDocument _uiDocument;

        private Button _exitButton1;
        private Button _exitButton2;
        private Button _purchaseBtn;
        private WarriorsFundPurchasePopupPresenter _presenter;
        private IAudioService _audioService;

        private PopupAppearanceAnimationToolkit _animation;

        private IMyLogger _logger;

        public void Construct(
            WarriorsFundPurchasePopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _audioService = audioService;
            _logger = logger;
            _presenter = presenter;
        }

        public void Initialize()
        {
            _presenter.Initialize();

            _exitButton1 = _uiDocument.rootVisualElement.Q<Button>("Exit-btn-bg");
            _exitButton2 = _uiDocument.rootVisualElement.Q<Button>("Exit-btn");
            _purchaseBtn = _uiDocument.rootVisualElement.Q<Button>("Common-purchase-btn");

            VisualElement popupContainer = _uiDocument.rootVisualElement.Q<VisualElement>("Popup-container");

            if (popupContainer != null)
            {
                _animation = new PopupAppearanceAnimationToolkit(popupContainer);
            }

            if (_exitButton1 != null)
                _exitButton1.clicked += OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked += OnExitButtonClicked;
            if (_purchaseBtn != null)
                _purchaseBtn.clicked += OnPurchaseButtonClicked;
            
            _presenter.Purchased += OnPurchased;

            Product product = _presenter.GetSegmentedIAP();

            if (product != null && _purchaseBtn != null)
            {
                _purchaseBtn.text = product.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture);
                _purchaseBtn.SetEnabled(true);
                return;
            }

            _purchaseBtn?.SetEnabled(false);
        }

        private void OnPurchased()
        {
            _logger.Log("ON WF PURCHASED", DebugStatus.Info);
            Hide();
        }

        private void Hide()
        {
            if (_animation != null)
            {
                _animation.Hide(OnHideComplete);
                return;
            }

            _logger.Log("HIDE WF PURCHASE POPUP WITHOUT ANIMATION", DebugStatus.Info);

            SetActive(false);
        }

        private void OnHideComplete()
        {
            _logger.Log("HIDE WF PURCHASE POPUP WITH ANIMATION", DebugStatus.Info);
            SetActive(false);
        }

        public void Show()
        {
            if (_animation != null)
            {
                _logger.Log("SHOW WF PURCHASE POPUP WITH ANIMATION", DebugStatus.Info);
                _animation.Show(OnShowComplete);
                return;
            }

            _logger.Log("SHOW WF PURCHASE POPUP WITHOUT ANIMATION", DebugStatus.Info);

            SetButtonsActive(true);
            SetActive(true);
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                return;
            }

            _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        public void SetDocumentActive(bool isActive)
        {
            _uiDocument.enabled = isActive;
        }

        private void SetButtonsActive(bool isActive)
        {
            if (_exitButton1 != null)
                _exitButton1.SetEnabled(isActive);
            if (_exitButton2 != null)
                _exitButton2.SetEnabled(isActive);
            if (_purchaseBtn != null)
                _purchaseBtn.SetEnabled(isActive);
        }

        private void OnShowComplete()
        {
            SetButtonsActive(true);
            SetActive(true);
        }

        public void Dispose()
        {
            _presenter.Purchased -= OnPurchased;

            _presenter.Dispose();

            if (_exitButton1 != null)
                _exitButton1.clicked -= OnExitButtonClicked;
            if (_exitButton2 != null)
                _exitButton2.clicked -= OnExitButtonClicked;
            if (_purchaseBtn != null)
                _purchaseBtn.clicked -= OnPurchaseButtonClicked;
            
        }

        private void OnPurchaseButtonClicked()
        {
            PlayButtonSound();
            _presenter.OnPurchaseButtonClicked();
        }

        private void OnExitButtonClicked()
        {
            PlayButtonSound();
            SetButtonsActive(false);
            Hide();
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}