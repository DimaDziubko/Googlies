using System;
using _Game.Common;
using _Game.Core.Ads;
using _Game.Core.Boosts;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay.Common;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonPopupPresenter
    {
        public event Action StateChanged;
        
        private readonly DungeonStrategyFactory _dungeonFactory;
        private readonly IDungeonModel _model;
        private readonly IAdsService _adsService;

        public DungeonPopupPresenter(
            DungeonStrategyFactory dungeonFactory,
            IDungeonModel model,
            IAdsService adsService)
        {
            _dungeonFactory = dungeonFactory;
            _model = model;
            _adsService = adsService;
            Initialize();
        }

        private void OnLevelChanged() => StateChanged?.Invoke();

        public void Initialize()
        {
            _model.LevelChanged += OnLevelChanged;
            _adsService.RewardedVideoLoaded += OnVideoLoaded;
        }

        public void Dispose()
        {
            _model.LevelChanged -= OnLevelChanged;
            _adsService.RewardedVideoLoaded -= OnVideoLoaded;
        }

        private void OnVideoLoaded() => StateChanged?.Invoke();

        public string GetDungeonName() =>_model.Name;

        public Sprite GetRewardIcon() => _model.RewardIcon;

        public string GetRewardAmount() => _model.GetRewardAmount.ToCompactFormat();

        public Sprite GetIcon()
        {
            if (_model.KeysCount > 0 || _model.VideosCount == 0)
            {
                return _model.KeyIcon;
            }

            return _model.AdsIcon;
        }

        public string GetBalance()
        {
            if (_model.KeysCount > 0)
            {
                return $"{_model.KeysCount}/{_model.MaxKeysCount}";
            }

            return $"{_model.VideosCount}/{_model.MaxVideosCount}";
        }

        public void OnEnter()
        {
            var strategy =  _dungeonFactory.GetStrategy(_model.DungeonType);
            
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
            var strategy =  _dungeonFactory.GetStrategy(_model.DungeonType);
            _model.SpendVideo();
            _model.AddKey(ItemSource.Ad);
            strategy.Execute();
        }

        public string GetDifficulty() => $"{_model.Stage}-{_model.SubLevel}";
        public bool CanMoveNext() => _model.CanMoveNext();
        public bool CanMovPrevious() => _model.CanMovePrevious();
        public void MoveNext() => _model.MoveToNextLevel();
        public void MovePrevious() => _model.MoveToPreviousLevel();

        public bool CanEnter()
        {
            if (_model.KeysCount > 0)
                return true;
            return _model.VideosCount > 0 && _adsService.IsAdReady(AdType.Rewarded);
        }
    }
}