using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Boosts;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.Common;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.Utils.Extensions;
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
        private readonly DungeonStrategyFactory _dungeonFactory;
        private readonly IMyLogger _logger;

        private AmountView _amountView;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        public DungeonView View => _view;

        public DungeonPresenter(
            IDungeonModel model,
            DungeonView view,
            DungeonStrategyFactory dungeonFactory,
            IUserContainer userContainer,
            IAdsService adsService,
            IAudioService audioService,
            IMyLogger logger
            )
        {
            _model = model;
            _view = view;
            _userContainer = userContainer;
            _adsService = adsService;
            _audioService = audioService;
            _dungeonFactory = dungeonFactory;
            _logger = logger;

            _amountView = _view.CostAmountView;
        }

        public void Initialize()
        {
            bool isLocked = _model.RequiredTimeline > TimelineState.TimelineNumber || _model.IsLocked;
            _view.SetLocked(isLocked);
            _view.SetName(_model.Name);
            _view.SetMainIcon(_model.Icon);

            _model.Dungeon.KeysCountChanged += UpdateState;
            _model.Dungeon.VideosCountChanged += UpdateState;
            _view.Clicked += OnEntered;
            _view.PreviousDifficultyBtn.onClick.AddListener(OnPreviousClicked);
            _view.NextDifficultyBtn.onClick.AddListener(OnNextClicked);

            UpdateState();
        }

        public void SetModel(IDungeonModel dungeonModel) => _model = dungeonModel;

        public void SetView(DungeonView view) => _view = view;

        private void UpdateState()
        {
            if (_view == null) return;

            _view.Difficulty.text = GetDifficulty();
            _view.RewardAmountView.SetIcon(_model.RewardIcon);
            _view.RewardAmountView.SetAmount(_model.GetRewardAmount.ToCompactFormat());
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
            }

            _view.NextDifficultyBtn.interactable = _model.CanMoveNext();
            _view.PreviousDifficultyBtn.interactable = _model.CanMovePrevious();
        }

        public void Dispose()
        {
            _model.Dungeon.KeysCountChanged -= UpdateState;
            _model.Dungeon.VideosCountChanged -= UpdateState;
            _view.Clicked -= OnEntered;
        }

        private async void OnEntered()
        {
            var strategy = _dungeonFactory.GetStrategy(_model.DungeonType);

            if (_model.KeysCount > 0)
            {
                strategy.Execute();
            }
            else if (_model.VideosCount > 0 && _adsService.IsAdReady(AdType.Rewarded))
            {
                _adsService.ShowRewardedVideo(OnVideoCompleted, Placement.Dungeon);
            }
        }

        private void OnVideoCompleted()
        {
            var strategy = _dungeonFactory.GetStrategy(_model.DungeonType);
            _model.SpendVideo();
            _model.AddKey(ItemSource.Ad);
            strategy.Execute();
        }

        private void OnNextClicked()
        {
            if (CanMoveNext())
            {
                MoveNext();
            }

            _audioService.PlayButtonSound();
            UpdateState();
        }

        private void OnPreviousClicked()
        {
            if (CanMovPrevious())
            {
                MovePrevious();
            }

            _audioService.PlayButtonSound();
            UpdateState();
        }

        public string GetDifficulty() => $"{_model.Stage}-{_model.SubLevel}";
        private bool CanMovPrevious() => _model.CanMovePrevious();
        private bool CanMoveNext() => _model.CanMoveNext();
        private void MoveNext() => _model.MoveToNextLevel();
        private void MovePrevious() => _model.MoveToPreviousLevel();

        public sealed class Factory : PlaceholderFactory<IDungeonModel, DungeonView, DungeonPresenter>
        {

        }
    }
}