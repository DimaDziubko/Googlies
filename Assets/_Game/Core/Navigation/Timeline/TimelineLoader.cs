using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;

namespace _Game.Core.Navigation.Timeline
{
    public interface ITimelineLoader
    {
        void LoadNextTimeline(int timelineId, Action onCompleted);
        void LoadPreviousTimeline(int timelineId, Action onCompleted);
    }

    public class TimelineLoader : ITimelineLoader
    {
        private readonly ILoadingScreenProvider _loadingScreenProvider;
        private readonly LoadingOperationFactory _loadingOperationFactory;
        private readonly ITimelineConfigRepository _config;
        private readonly IMyLogger _logger;

        public TimelineLoader(
            ILoadingScreenProvider loadingScreenProvider,
            LoadingOperationFactory loadingOperationFactory,
            IConfigRepository config,
            IMyLogger logger)
        {
            _logger = logger;
            _loadingScreenProvider = loadingScreenProvider;
            _loadingOperationFactory = loadingOperationFactory;
            _config = config.TimelineConfigRepository;
        }

        public void LoadNextTimeline(int timelineId, Action onCompleted)
        {
            Queue<ILoadingOperation> operations = PrepareTimelineOperations(timelineId, isNext: true);
            ExecuteLoadingOperations(operations, onCompleted);
        }

        public void LoadPreviousTimeline(int timelineId, Action onCompleted)
        {
            Queue<ILoadingOperation> operations = PrepareTimelineOperations(timelineId, isNext: false);
            ExecuteLoadingOperations(operations, onCompleted);
        }

        private Queue<ILoadingOperation> PrepareTimelineOperations(int timelineId, bool isNext)
        {
            Queue<ILoadingOperation> operations = new Queue<ILoadingOperation>();

            int timelineIdToRelease = isNext ? timelineId - 1 : timelineId + 1;

            var iconsToLoad = _config.GetAgeConfigs(timelineId)
                .Select(x => x.GetIconReference()).ToList();

            var ambienceToLoad = _config.GetBattleConfigs(timelineId)
                .Select(x => x.AmbienceKey).ToList();

            var warriorsIconsToLoad = new List<IIconReference>();

            warriorsIconsToLoad.AddRange(
                _config.GetRelatedAgeWarriors(timelineId, 0)
                    .Select(x => x.GetIconReferenceFor(Skin.Ally))
            );

            warriorsIconsToLoad.AddRange(
                _config.GetAllBattleWarriors(timelineId)
                    .Select(x => x.GetIconReferenceFor(Skin.Hostile))
            );
            
            Queue<ILoadingOperation> parallelLoadingOperations = new Queue<ILoadingOperation>();
            
            parallelLoadingOperations.Enqueue(_loadingOperationFactory.CreateAgeIconsLoadingOperation(iconsToLoad));
            parallelLoadingOperations.Enqueue(_loadingOperationFactory.CreateAmbienceLoadingOperation(ambienceToLoad));
            parallelLoadingOperations.Enqueue(_loadingOperationFactory.CreateWarriorIconsLoadingOperation(warriorsIconsToLoad));
            
            operations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Loading timeline resources",parallelLoadingOperations));
            
            if (timelineIdToRelease >= 0)
            {
                var iconsToRelease = _config.GetAgeConfigs(timelineIdToRelease)
                    .Select(x => x.GetIconReference()).ToList();

                var ambienceToRelease = _config.GetBattleConfigs(timelineIdToRelease)
                    .Select(x => x.AmbienceKey).ToList();

                var warriorsIconsToRelease = new List<IIconReference>();

                warriorsIconsToRelease.AddRange(
                    _config.GetAllAgeWarriors(timelineIdToRelease)
                        .Select(x => x.GetIconReferenceFor(Skin.Ally))
                );

                warriorsIconsToRelease.AddRange(
                    _config.GetAllBattleWarriors(timelineIdToRelease)
                        .Select(x => x.GetIconReferenceFor(Skin.Hostile))
                );

                Queue<ILoadingOperation> parallelClearingOperations = new Queue<ILoadingOperation>();
                
                parallelClearingOperations.Enqueue(_loadingOperationFactory.CreateBaseClearingOperation());
                parallelClearingOperations.Enqueue(_loadingOperationFactory.CreateEnvironmentClearingOperation());
                parallelClearingOperations.Enqueue(_loadingOperationFactory.CreateAgeIconsReleasingOperation(iconsToRelease));
                parallelClearingOperations.Enqueue(_loadingOperationFactory.CreateAmbienceReleasingOperation(ambienceToRelease));
                parallelClearingOperations.Enqueue(_loadingOperationFactory.CreateWarriorIconsReleasingOperation(warriorsIconsToRelease));
            
                operations.Enqueue(_loadingOperationFactory.CreateParallelOperation("Clearing timeline resources",parallelClearingOperations));
            }

            return operations;
        }

        private void ExecuteLoadingOperations(Queue<ILoadingOperation> operations, Action onCompleted)
        {
            _loadingScreenProvider.LoadingCompleted += OnLoadingCompleted;
            _loadingScreenProvider.LoadAndDestroy(operations, LoadingScreenType.Transparent);

            void OnLoadingCompleted()
            {
                _loadingScreenProvider.LoadingCompleted -= OnLoadingCompleted;
                onCompleted?.Invoke();
            }
        }
    }
}