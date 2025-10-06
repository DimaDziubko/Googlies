using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.Utils.Disposable;
using Zenject;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonPresenter
    {
        private IDungeonModel _model;
        private DungeonView _view;
        private readonly IUserContainer _userContainer;
        private readonly IAdsService _adsService;
        private readonly IAudioService _audioService;
        
        private AmountView _amountView;

        private DungeonPopupPresenter _presenter;
        private readonly DungeonStrategyFactory _dungeonFactory;
        private IDungeonPopupProvider _provider;
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;
        
        private Disposable<DungeonPopup> _popup;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        public DungeonView View => _view;

        public DungeonPresenter(
            IDungeonModel model, 
            DungeonView view,
            DungeonStrategyFactory dungeonFactory,
            IUserContainer userContainer,
            IAdsService adsService,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger)
        {
            _model = model;
            _view = view;
            _userContainer = userContainer;
            _adsService = adsService;
            _cameraService = cameraService;
            _audioService = audioService;
            _dungeonFactory = dungeonFactory;
            _logger = logger;
        }

        public void Initialize()
        {
            bool isLocked = _model.RequiredTimeline > TimelineState.TimelineNumber || _model.IsLocked;
            _view.SetLocked(isLocked);
            _view.SetName(_model.Name);
            _view.SetMainIcon(_model.Icon);
            _view.SetTimeline($"Timeline {_model.RequiredTimeline}");
            
            _view.SetRewardIcon(_model.RewardIcon);
            
            _model.Dungeon.KeysCountChanged += UpdateState;
            _model.Dungeon.VideosCountChanged += UpdateState;
            _view.Clicked += OnEntered;

            _presenter?.Initialize();
            
            UpdateState();
        }

        public void OnScreeActiveChanged(bool isActive) => 
            _popup?.Value.SetActive(isActive);

        public void SetModel(IDungeonModel dungeonModel) => _model = dungeonModel;

        public void SetView(DungeonView view) => _view = view;

        private void UpdateState()
        {
            if(_amountView == null)
                _amountView = _view.AmountListView.SpawnElement();

            if (_model.KeysCount == 0 && _model.VideosCount != 0 && _adsService.IsAdReady(AdType.Rewarded))
            {
                _amountView.SetIcon(_model.AdsIcon);
                _amountView.SetAmount($"{_model.VideosCount}/{_model.MaxVideosCount}");
                _view.SetInteractable(true);
            }
            else
            {
                _amountView.SetIcon(_model.KeyIcon);
                _amountView.SetAmount($"{_model.KeysCount}/{_model.MaxKeysCount}");
                _view.SetInteractable(true);
                //_view.SetInteractable(_model.KeysCount > 0);
            }
        }

        public void Dispose()
        {
            _model.Dungeon.KeysCountChanged -= UpdateState;
            _model.Dungeon.VideosCountChanged -= UpdateState;
            _view.Clicked -= OnEntered;
            _presenter?.Dispose();
        }

        private async void OnEntered()
        {
            _audioService.PlayButtonSound();
            
            _presenter ??= new DungeonPopupPresenter(_dungeonFactory, _model, _adsService);
            _provider ??= new DungeonPopupProvider(_cameraService, _audioService, _presenter);
            
            _popup  = await _provider.Load();
            var isConfirmed = await _popup.Value.AwaitForDecision();
            if (!isConfirmed)
            {
                _popup.Value.Dispose();
                _popup.Dispose();
                _popup = null;
            }
        }

        public sealed class Factory : PlaceholderFactory<IDungeonModel, DungeonView, DungeonPresenter>
        {

        }
    }
}