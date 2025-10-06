using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Theme.Binders;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;

namespace _Game.UI.Settings.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class SettingsPopup : MonoBehaviour
    {
        //TODO: Adjust
        private readonly string _emailAddress = "support@playcus.com";
        private readonly string _subjectMail = "TheArmyEvolution Feedback";
        private readonly string _privacyURL = "https://www.playcus.com/privacy-policy";

        [SerializeField, Required] private Canvas _canvas;

        [SerializeField, Required] private Button[] _cancelButtons;

        [SerializeField, Required] private CustomToggle _sfxToggle, _ambienceToggle, _damageTextToggle;
        [SerializeField, Required] private Button _rateUs;
        [SerializeField, Required] private Button _privacyButton;
        [SerializeField, Required] private Button _writeUsButton;
        [SerializeField, Required] private Button _restoreBtn;
        [SerializeField, Required] private RestoreHintPopup _restoreHintPopup;
        [SerializeField, Required] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private ISettingsSaveGameReadonly Settings => _userContainer.State.SettingsSaveGame;

        private IAudioService _audioService;
        private IUserContainer _userContainer;
        private IIAPService _iapService;

        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IIAPService iapService,
            IUserContainer userContainer)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _iapService = iapService;
            _userContainer = userContainer;

            InitializeUIElements();
        }

        private void InitializeUIElements()
        {
#if UNITY_ANDROID
            _restoreBtn.gameObject.SetActive(false);
#endif
            Unsubscribe();
            Subscribe();
            _sfxToggle.Initialize(_audioService.IsOnSFX(), _audioService);
            _ambienceToggle.Initialize(_audioService.IsOnAmbience(), _audioService);
            _damageTextToggle.Initialize(Settings.IsDamageTextOn, _audioService);
            _restoreHintPopup.Hide();
            
        }


        public async UniTask<bool> AwaitForExit()
        {
            _animation.PlayShow(OnShowComplete);

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void OnCloseBtnClick()
        {
            _restoreHintPopup.Cleanup();
            DisableButtons();
            _audioService.PlayButtonSound();
            _animation.PlayHide(OnHideCompleteAccept);
        }

        private void DisableButtons()
        {
            foreach (var button in _cancelButtons)
            {
                button.interactable = false;
            }

            _sfxToggle.SetInteractable(false);
            _ambienceToggle.SetInteractable(false);
            _damageTextToggle.SetInteractable(false);
            _rateUs.interactable = false;
            _privacyButton.interactable = false;
            _writeUsButton.interactable = false;
            _restoreBtn.interactable = false;
        }

        private void OnHideCompleteAccept()
        {
            Unsubscribe();
            _sfxToggle.Cleanup();
            _ambienceToggle.Cleanup();
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }

        private void Subscribe()
        {
            _sfxToggle.ValueChanged += _audioService.SwitchSFX;
            _ambienceToggle.ValueChanged += _audioService.SwitchAmbience;
            _damageTextToggle.ValueChanged += OnDamageTextSwitched;
#if UNITY_ANDROID
            _restoreBtn.onClick.AddListener(_iapService.RestorePurchasesAndroid);
#elif UNITY_IOS
            _restoreBtn.onClick.AddListener(_iapService.RestorePurchasesIOS);
#endif
            _iapService.Restored += OnRestored;

            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCloseBtnClick);
            }

            _privacyButton.onClick.AddListener(OnOpenPrivacy);
            _writeUsButton.onClick.AddListener(OnOpenEmailApp);
            _rateUs.onClick.AddListener(RateGame);
        }

        private void Unsubscribe()
        {
            _damageTextToggle.ValueChanged -= OnDamageTextSwitched;
            _sfxToggle.ValueChanged -= _audioService.SwitchSFX;
            _ambienceToggle.ValueChanged -= _audioService.SwitchAmbience;
            _restoreBtn.onClick.RemoveAllListeners();
            _iapService.Restored -= OnRestored;

            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveListener(OnCloseBtnClick);
            }
            _privacyButton.onClick.RemoveAllListeners();
            _writeUsButton.onClick.RemoveAllListeners();
            _rateUs.onClick.RemoveAllListeners();
        }

        private void OnDamageTextSwitched(bool isActive) =>
            _userContainer.State.SettingsSaveGame.SetDamageTextActive(isActive);

        private void OnRestored()
        {
            _restoreHintPopup.Show();
        }
        
        private void OnOpenEmailApp()
        {
            // Build the email body with the required information
            string emailBody = BuildEmailBody();

#if UNITY_IOS
            OpenEmailAppUtill.OpenEmailApp(emailBody, _emailAddress, _subjectMail);
#elif UNITY_ANDROID
            OpenEmailAppUtill.OpenEmailApp(emailBody, _emailAddress, _subjectMail);
#else
    // For other platforms
    string emailTo = "mailto:" + _emailAddress +
                     "?subject=" + Uri.EscapeDataString(_subjectMail) +
                     "&body=" + Uri.EscapeDataString(emailBody);
    Application.OpenURL(emailTo);
#endif
        }
        private string BuildEmailBody()
        {
            // Collect device and app information
            string deviceName = SystemInfo.deviceName;
            string deviceModel = SystemInfo.deviceModel;
            string operatingSystem = SystemInfo.operatingSystem;
            string appVersion = Application.version;
            string deviceID = SystemInfo.deviceUniqueIdentifier;

            // Build the email body
            string emailBody = "1.Describe your problem please:\n\n" +
                               "\n" +
                               $"Device Name: {deviceName}\n" +
                               $"Device Model: {deviceModel}\n" +
                               $"Operating System: {operatingSystem}\n" +
                               $"App Version: {appVersion}\n" +
                               $"Device ID: {deviceID}\n" +
                               $"TimelineID: {TimelineState.TimelineId + 1}\n" +
                               $"AgeID: {TimelineState.AgeId + 1}\n" +
                               $"BattleID: {TimelineState.MaxBattle + 1}\n" +
                               $"Level: {TimelineState.Level}\n";

            return emailBody;
        }
        private void OnOpenPrivacy()
        {
            Application.OpenURL(_privacyURL);
        }
        private void RateGame()
        {
#if UNITY_IOS
            Device.RequestStoreReview();
#elif UNITY_ANDROID
            DirectlyOpenRateGame();
#endif
        }
        private void DirectlyOpenRateGame() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }

    }
}