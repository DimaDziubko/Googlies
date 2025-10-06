using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.UI._Hud._BattleSpeedView;
using _Game.UI._Hud._CoinCounterView;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._Hud._FoodBoostView;
using _Game.UI._Hud._SpeedBoostView.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class BattleHud : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private CoinCounterView _counterView;
        [SerializeField] private ToggleWithSpriteSwap _pauseView;
        [SerializeField] private BattleSpeedView _battleSpeedView;
        [SerializeField] private SpeedBoostView _speedBoostView;
        [SerializeField] private DailyTaskView _dailyTaskView;
        [SerializeField] private WaveInfoPopup _waveInfo;
        [SerializeField] private FoodBoostBtn _foodBoostBtn;
        public CoinCounterView CounterView => _counterView;
        public ToggleWithSpriteSwap PauseView => _pauseView;
        public CoinCounterView CoinCounterView => _counterView;
        public WaveInfoPopup WaveInfoPopup => _waveInfo;
        public FoodBoostBtn FoodBoostBtn => _foodBoostBtn;
        public DailyTaskView DailyTaskView => _dailyTaskView;
        
        public void Construct(
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
        }

        public void Init() => Show();

        private void Show() => _canvas.enabled = true;

        public void Hide() => _canvas.enabled = false;
        
    }
}