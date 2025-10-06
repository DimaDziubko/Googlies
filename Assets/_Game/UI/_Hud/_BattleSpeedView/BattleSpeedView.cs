using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.UI._Hud._BattleSpeedView
{
    public class BattleSpeedView : MonoBehaviour
    {
        [SerializeField] private BattleSpeedBtn _battleSpeedBtn;

        private IBattleSpeedService _battleSpeed;
        private IAudioService _audioService;

        public void Construct(
            IBattleSpeedService battleSpeedService,
            IAudioService audioService)
        {
            _battleSpeed = battleSpeedService;
            _audioService = audioService;
            Show();
        }
        
        private void Show()
        {
            //SubscribeBattleSpeedButton();
            _battleSpeedBtn.Initialize(OnBattleSpeedBtnClicked);
            OnBattleSpeedBtnShown();
        }

        //public void Hide() => UnsubscribeBattleSpeedButton();
        
        private void OnBattleSpeedBtnShown() => 
            _battleSpeed.OnBattleSpeedBtnShown();

        // private void SubscribeBattleSpeedButton()
        // {
        //     _battleSpeed.SpeedBoostTimerActivityChanged += OnTimerChanged;
        //     _battleSpeed.BattleSpeedBtnModelChanged += _battleSpeedBtn.UpdateBtnState;
        // }

        // private void OnTimerChanged(GameTimer timer, bool isActive)
        // {
        //     if (isActive)
        //     {
        //         timer.Tick += OnBattleSpeedTimerTick;
        //         return;
        //     }
        //     
        //     timer.Tick -= OnBattleSpeedTimerTick;
        // }

        // private void UnsubscribeBattleSpeedButton()
        // {
        //     _battleSpeed.SpeedBoostTimerActivityChanged -= OnTimerChanged;
        //     _battleSpeed.BattleSpeedBtnModelChanged -= _battleSpeedBtn.UpdateBtnState;
        // }

        private void OnBattleSpeedTimerTick(float timeLeft) => 
            _battleSpeedBtn.UpdateTimer(timeLeft);


        private void OnBattleSpeedBtnClicked()
        {
            _audioService.PlayButtonSound();
            _battleSpeed.OnBattleSpeedBtnClicked();
        }
    }
}