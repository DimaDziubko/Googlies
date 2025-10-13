using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.Utils.Extensions;

namespace _Game.UI._TimelineInfoPresenter
{
    public class TimelineInfoPresenter : ITimelineInfoPresenter, IDisposable
    {
        public event Action StateChanged;
        public IEnumerable<TimelineInfoItem> Items => _items;
        public int CurrentAge => _ageNavigator.CurrentIdx;
        public int NextAge => _ageNavigator.CurrentIdx + 1;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IMyLogger _logger;
        private readonly IAgeNavigator _ageNavigator;
        private readonly AgeIconContainer _container;

        private List<TimelineInfoItem> _items;


        private readonly IConfigRepository _configRepository;
        private ITimelineConfigRepository TimelineConfig => _configRepository.TimelineConfigRepository;
        private IDifficultyConfigRepository Difficulty => _configRepository.DifficultyConfigRepository;

        public TimelineInfoPresenter(
            IAgeNavigator ageNavigator,
            IMyLogger logger,
            IGameInitializer gameInitializer,
            ITimelineNavigator timelineNavigator,
            IConfigRepository configRepository,
            AgeIconContainer container)
        {
            _configRepository = configRepository;
            _ageNavigator = ageNavigator;
            _logger = logger;
            _gameInitializer = gameInitializer;
            _timelineNavigator = timelineNavigator;
            _container = container;

            gameInitializer.OnPostInitialization += Init;
        }

        public string GetDifficulty()
        {
            return $"Difficulty x{Difficulty.GetDifficultyValue(_timelineNavigator.CurrentTimelineId).ToCompactFormat()}";
        }

        public string GetTimelineText() =>
            $"Timeline {_timelineNavigator.CurrentTimelineNumber.ToString()}";

        private void Init()
        {
            PrepareTimelineInfoItems();
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            _ageNavigator.AgeChanged += OnNextAgeOpened;
        }

        void IDisposable.Dispose()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _ageNavigator.AgeChanged -= OnNextAgeOpened;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnNextAgeOpened() => UpdateTimelineInfoItems();


        private void OnTimelineChanged()
        {
            PrepareTimelineInfoItems();
            UpdateTimelineInfoItems();
            StateChanged?.Invoke();
        }

        private void PrepareTimelineInfoItems()
        {
            var totalAgesCount = _ageNavigator.GetTotalAgesCount();

            _items = new List<TimelineInfoItem>(totalAgesCount);

            int ageIndex = 0;

            for (int i = 0; i < totalAgesCount; i++)
            {
                var ageConfig = TimelineConfig.GetRelatedAge(_timelineNavigator.CurrentTimelineId, ageIndex);

                var iconRef = ageConfig.GetIconReference();
                var icon = _container.Get(iconRef.Atlas.AssetGUID).Get(iconRef.IconName);

                var model = new TimelineInfoItem.TimelineInfoItemBuilder()
                    .WithIcon(icon)
                    .WithName(ageConfig.Name)
                    .WithDateRange(ageConfig.DateRange)
                    .WithDescription(ageConfig.Description)
                    .Build();

                model.SetLocked(ageIndex > NextAge);
                _items.Add(model);
                ageIndex++;
            }
        }

        private void UpdateTimelineInfoItems()
        {
            int ageIndex = 0;

            foreach (var item in _items)
            {
                item.SetLocked(ageIndex > CurrentAge);
                ageIndex++;
            }
        }
    }
}