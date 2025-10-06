using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.UI._Hud._SpeedBoostView.Scripts
{
    public class SpeedBoostView : MonoBehaviour
    {
        [SerializeField] private SpeedBoostBtn _speedBoostBtn;

        private ISpeedBoostService _speedBoost;
        private IAudioService _audioService;

        public void Construct(
            ISpeedBoostService speedBoost,
            IAudioService audioService)
        {
            _speedBoost = speedBoost;
            _audioService = audioService;
        }

        public void Init()
        {
            SubscribeSpeedBoostBtn();
            InitSpeedBoostBtn();
        }

        public void Show() => OnSpeedBoostBtnShown();

        public void Hide() { }

        private void SubscribeSpeedBoostBtn() =>
            _speedBoost.SpeedBoostBtnModelChanged += _speedBoostBtn.UpdateBtnState;

        private void UnsubscribeSpeedBoostBtn() =>
            _speedBoost.SpeedBoostBtnModelChanged -= _speedBoostBtn.UpdateBtnState;

        private void InitSpeedBoostBtn() =>
            _speedBoostBtn.Initialize(OnSpeedBoostBtnClicked);

        private void OnSpeedBoostBtnShown() =>
            _speedBoost.OnSpeedBoostBtnShown();

        private void OnSpeedBoostBtnClicked()
        {
            _speedBoost.OnSpeedBoostBtnClicked();
            _audioService.PlayButtonSound();
        }

        public void Cleanup() => UnsubscribeSpeedBoostBtn();
    }
}