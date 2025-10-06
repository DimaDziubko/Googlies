using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._GameEventRouter;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.Common;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.Utils;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.LiveopsCore._GameEventStrategies
{
    public class WarriorsFundStrategy : IGameEventStrategy
    {
        public event Action<GameEventBase> Complete;
        
        private readonly WarriorsFundEvent _event;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly GameEventRouter _gameEventRouter;
        private readonly ICoroutineRunner _runner;

        private readonly WarriorsFundPurchaseProcessor _purchaseProcessor;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private readonly Dictionary<IRewardItem, Coroutine> _rewardCoroutines = new();

        private bool _isInitialized;
        
        public WarriorsFundStrategy(
            WarriorsFundEvent @event,
            RewardProcessingService rewardProcessingService,
            GameEventRouter gameEventRouter,
            IUserContainer userContainer,
            WarriorsFundPurchaseProcessor purchaseProcessor,
            ICoroutineRunner runner,
            IMyLogger logger)
        {
            _runner = runner;
            _gameEventRouter = gameEventRouter;
            _userContainer = userContainer;
            _rewardProcessingService = rewardProcessingService;
            _event = @event;
            _logger = logger;
            _purchaseProcessor = purchaseProcessor;
        }
        
        void IGameEventStrategy.Execute()
        {
            if (_event.Points != null)
            {
                Init();
            }
                
            _event.PointsInitialized += Init;
        }

        private void Init()
        {
            if(_isInitialized) return;
            
            _gameEventRouter.TimelineChanged += OnProgressChanged;
            _gameEventRouter.AgeChanged += OnProgressChanged;
            
            _purchaseProcessor.Initialize();
            
            ApplyPointsState((p, ready) => p.SetReady(ready));

            foreach (var point in _event.Points)
            {
                point.FreePoint.OnClaimRequestedDelayed += OnClaimRequestDelayed;
                point.PremiumPoint.OnClaimRequestedDelayed += OnClaimRequestDelayed;
            }

            _isInitialized = true;
        }

        private void OnClaimRequestDelayed(IRewardItem point, float delay)
        {
            _rewardCoroutines[point] = _runner.StartCoroutine(ClaimAfterDelay(point, delay));
        }

        private IEnumerator ClaimAfterDelay(IRewardItem point, float delay)
        {
            point.ChangeClaimed(true);
            yield return new WaitForSeconds(delay);
            _rewardCoroutines[point] = null;
            _rewardCoroutines.Remove(point);
            _rewardProcessingService.Process(point, ItemSource.LeaderPass);
        }

        private void OnProgressChanged(int _)
        {
            ApplyPointsState((p, ready) => p.ChangeReady(ready));
            
            if (_isInitialized && AllReady()) _event.NotifyCompleted();
        }

        private bool AllReady()
        {
            if (_event.Points != null && _event.Points.All(x => x.IsReady))
            {
                return true;
            }
            
            return false;
        }

        private void ApplyPointsState(Action<WarriorsFundPointGroup, bool> applyReady)
        {
            if (_event.Points == null || _event.Points.Count == 0)
            {
                UpdateRoadProgress();
                return;
            }
            
            foreach (var point in _event.Points)
            {
                bool isReady = point.Objective.CanPass(TimelineState.TimelineNumber, TimelineState.AgeNumber, TimelineState.BattleNumber);
                applyReady(point, isReady);
            
                _logger.Log(
                    $"POINT {(isReady ? "READY" : "NOT READY")} | OBJECTIVE {point.Objective}",
                    DebugStatus.Info);
            }
            
            UpdateRoadProgress(TimelineState.TimelineNumber, TimelineState.AgeNumber);
        }

        private void UpdateRoadProgress(int timelineNumber = 1, int ageNumber = 1)
        {
            var firsPoint = _event.Points.FirstOrDefault();
            var lastPoint = _event.Points.LastOrDefault();

            if (_event.Points == null || firsPoint == null || lastPoint == null)
            {
                _event.ChangeRoadProgress(0);
                return;
            }
            
            if (firsPoint.Objective.TimelineNumber >= timelineNumber && firsPoint.Objective.AgeNumber >= ageNumber)
            {
                _event.ChangeRoadProgress(0);
                return;
            }
            if (lastPoint.Objective.TimelineNumber <= timelineNumber && lastPoint.Objective.AgeNumber <= ageNumber)
            {
                _event.ChangeRoadProgress(1);
                return;
            }
            
            IReadOnlyList<WarriorsFundPointGroup> points = _event.Points;
            
            int currentAgeSerialNumber = (timelineNumber - 1) * Constants.Config.AGES_IN_TIMELINE + ageNumber;
            
            float segmentSize = (float) 1 / (points.Count - 1);

            for (int i = 0, j = 1; j < points.Count; i++, j++)
            {
                var nextPoint = points[j];
                var previousPoint = points[i];
                
                int nextPointAgeSerialNumber =
                    nextPoint.Objective.GetAgeSerialNumber(Constants.Config.AGES_IN_TIMELINE);
                
                int previousPointAgeSerialNumber =
                    previousPoint.Objective.GetAgeSerialNumber(Constants.Config.AGES_IN_TIMELINE);
                
                if (nextPointAgeSerialNumber > currentAgeSerialNumber)
                {
                    float amountPerSegmentUnit = segmentSize / (nextPointAgeSerialNumber - previousPointAgeSerialNumber);
                    
                    var roadProgress =
                        Mathf.Clamp01(segmentSize * i + amountPerSegmentUnit * (currentAgeSerialNumber - previousPointAgeSerialNumber));
                    
                    _event.ChangeRoadProgress(roadProgress);
                    return;
                }
            }
        }

        void IGameEventStrategy.UnExecute() => CheckForUnclaimedEventRewards("UNEXECUTED");

        private void CheckForUnclaimedEventRewards(string from)
        {
            _logger.Log($"CHECK REWARDS FROM {from}");
            
            Dictionary<int, IRewardItem> unclaimedRewards = new Dictionary<int, IRewardItem>();
            
            foreach (var point in _event.Points)
            {
                if (point.FreePoint.IsRewardReady && !point.FreePoint.IsRewardClaimed)
                {
                    TryAddUnclaimedReward(unclaimedRewards, point.FreePoint);
                }
                
                if (point.PremiumPoint.IsRewardReady && !point.PremiumPoint.IsRewardClaimed && !point.PremiumPoint.IsLocked)
                {
                    TryAddUnclaimedReward(unclaimedRewards, point.PremiumPoint);
                }
            }
            
            _event.SetUnclaimedRewards(unclaimedRewards.Values.ToList());
        }
        
        private void TryAddUnclaimedReward(Dictionary<int, IRewardItem> unclaimedRewards, IRewardItem reward)
        {
            if (reward == null || reward.IsRewardClaimed) return;

            if (!unclaimedRewards.ContainsKey(reward.Id))
            {
                unclaimedRewards.Add(reward.Id, new RewardItemModel(
                    new RewardItem
                    {
                        Id = reward.Id,
                        Amount = reward.Save.Amount,
                        IsRewardClaimed = false
                    },
                    reward.Details,
                    reward.Icon
                ));
            }
            else
            {
                unclaimedRewards[reward.Id].Save.AddAmount(reward.Save.Amount);
            }
        }

        void IGameEventStrategy.Cleanup()
        {
            _event.PointsInitialized -= Init;
            _gameEventRouter.TimelineChanged -= OnProgressChanged;
            _gameEventRouter.AgeChanged -= OnProgressChanged;
            _purchaseProcessor.Dispose();
            
            foreach (var point in _event.Points)
            {
                point.FreePoint.OnClaimRequestedDelayed -= OnClaimRequestDelayed;
                point.PremiumPoint.OnClaimRequestedDelayed -= OnClaimRequestDelayed;
            }
            
            foreach (var coroutine in _rewardCoroutines.Values)
            {
                if (coroutine != null)
                {
                    _runner.StopCoroutine(coroutine);
                }
            }
            
            _rewardCoroutines.Clear();
        }
    }
}