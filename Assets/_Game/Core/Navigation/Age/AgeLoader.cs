using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;

namespace _Game.Core.Navigation.Age
{
    public class AgeLoader : IAgeLoader
    {
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly LoadingOperationFactory _loadingOperationFactory;
        private readonly ITimelineConfigRepository _config;

        public AgeLoader(
            ILoadingScreenProvider loadingScreenProvider,
            LoadingOperationFactory loadingOperationFactory,
            IConfigRepository config)
        {
            _loadingScreenProvider = loadingScreenProvider;
            _loadingOperationFactory = loadingOperationFactory;
            _config = config.TimelineConfigRepository;
        }

        public void LoadNextAge(int timelineId, int ageIdx, Action onCompleted)
        {
            ExecuteLoading(timelineId, ageIdx, true, onCompleted);
        }

        public void LoadPreviousAge(int timelineId, int ageIdx, Action onCompleted)
        {
            ExecuteLoading(timelineId, ageIdx, false, onCompleted);
        }

        private void ExecuteLoading(int timelineId, int ageIdx, bool isNextAge, Action onCompleted)
        {
            Queue<ILoadingOperation> operations = new Queue<ILoadingOperation>();

            var iconsToLoad = _config.GetRelatedAgeWarriors(timelineId, ageIdx)
                .Select(x => x.GetIconReferenceFor(Skin.Ally));

            operations.Enqueue(_loadingOperationFactory.CreateWarriorIconsLoadingOperation(iconsToLoad));

            if (isNextAge)
            {
                if (ageIdx > 0)
                {
                    AddReleaseOperation(operations, timelineId, ageIdx - 1);
                }
                else if (timelineId > 0)
                {
                    AddReleaseOperation(operations, timelineId - 1, _config.LastAgeIdx());
                }
            }
            else
            {
                if (ageIdx < _config.LastAgeIdx())
                {
                    AddReleaseOperation(operations, timelineId, ageIdx + 1);
                }
                else
                {
                    AddReleaseOperation(operations, timelineId + 1, 0);
                }
            }

            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingScreenProvider.LoadAndDestroy(operations, LoadingScreenType.Transparent);

            void OnLoadingCompleted()
            {
                _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
                onCompleted?.Invoke();
            }
        }

        private void AddReleaseOperation(Queue<ILoadingOperation> operations, int timelineId, int ageIdx)
        {
            var iconsToRelease = _config.GetRelatedAgeWarriors(timelineId, ageIdx)
                .Select(x => x.GetIconReferenceFor(Skin.Ally));

            if (iconsToRelease.Any())
            {
                operations.Enqueue(_loadingOperationFactory.CreateWarriorIconsReleasingOperation(iconsToRelease));
            }
        }
    }
}