using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core.Boosts;
using _Game.Core.Services.IAP;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.Common;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.Utils.Timers;
using UnityEngine;

namespace _Game.LiveopsCore._GameEventStrategies
{
    public class BattlePassStrategy : IGameEventStrategy
    {
        public event Action<GameEventBase> Complete;
        
        private readonly BattlePassEvent _event;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly IIAPService _iapService;
        private readonly CurrencyBank _currencyBank;
        private readonly ICoroutineRunner _runner;
        private readonly IMyLogger _logger;
        
        private readonly BattlePassPurchaseProcessor _purchaseProcessor;

        private SynchronizedCountdownTimer _eventTimer;

        private CurrencyCell _pointsCell;

        private readonly Dictionary<IRewardItem, Coroutine> _rewardCoroutines = new();

        public BattlePassStrategy(
            BattlePassEvent @event,
            RewardProcessingService rewardProcessingService,
            IMyLogger logger,
            CurrencyBank currencyBank,
            ICoroutineRunner runner,
            BattlePassPurchaseProcessor purchaseProcessor)
        {
            _runner = runner;
            _event = @event;
            _rewardProcessingService = rewardProcessingService;
            _logger = logger;
            _currencyBank = currencyBank;

            _purchaseProcessor = purchaseProcessor;
        }

        void IGameEventStrategy.Execute()
        {
            _pointsCell = _currencyBank.GetCell(_event.CurrencyCellType);
            
            if (!_event.IsOnBreak)
            {
                _pointsCell.OnStateChanged += OnPointsStateChanged;
                InitPoints();
            }
            
            InitEventTimer();
            
            if (!_event.IsOnBreak)
            {
                foreach (var point in _event.Points)
                {
                    point.FreePoint.OnClaimRequestedDelayed += OnClaimRequestDelayed;
                    point.PremiumPoint.OnClaimRequestedDelayed += OnClaimRequestDelayed;
                }
            }
            
            _purchaseProcessor.Initialize();
            
            if (EventExpired()) OnEventTimerStop();
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

        void IGameEventStrategy.Cleanup()
        {
            _purchaseProcessor.Dispose();
            
            foreach (var coroutine in _rewardCoroutines.Values)
            {
                if (coroutine != null)
                {
                    _runner.StopCoroutine(coroutine);
                }
            }
            
            _rewardCoroutines.Clear();
            
            
            foreach (var point in _event.Points)
            {
                point.FreePoint.OnClaimRequestedDelayed -= OnClaimRequestDelayed;
                point.PremiumPoint.OnClaimRequestedDelayed -= OnClaimRequestDelayed;
            }
            
            if (_eventTimer != null)
            {
                _eventTimer.TimerStop -= OnEventTimerStop;
                _eventTimer.OnTick -= OnEventTimerTick;
                _eventTimer.Dispose();
            }
            
            _pointsCell.OnStateChanged -= OnPointsStateChanged;
        }

        private void InitPoints() =>
            ApplyPointsState(
                (p, ready) => p.SetReady(ready),
                () => SetPointProgress()
            );

        private void OnPointsStateChanged() =>
            ApplyPointsState(
                (p, ready) => p.ChangeReady(ready),
                () => UpdatePointProgress()
            );
        
        private void ApplyPointsState(
            Action<BattlePassPointGroup, bool> applyReady,
            Action applyProgress
        )
        {
            double amount = _pointsCell.Amount;
            
            if (_event.Points == null || _event.Points.Count == 0)
            {
                UpdateRoadProgress(0);
                applyProgress();
                return;
            }

            foreach (var point in _event.Points)
            {
                bool isReady = amount >= point.Objective;
                applyReady(point, isReady);

                _logger.Log(
                    $"POINT {(isReady ? "READY" : "NOT READY")} | LOCAL_POINTS {amount} | OBJECTIVE {point.Objective}",
                    DebugStatus.Info);

                amount -= point.Objective;
                if (amount < 0) amount = 0;
            }
            
            UpdateRoadProgress(_pointsCell.Amount);
            
            applyProgress();
        }
        
        private void ApplyPointProgress(Action<int> applyNextObjective)
        {
            var pointsAmount = _pointsCell.Amount;
            var points = _event.Points;
            int total = points.Count;

            if (total == 0)
            {
                applyNextObjective?.Invoke(0);
                _event.ChangeProgressValue(0f);
                _event.ChangeProgressText("0 / 0");
                return;
            }

            for (int i = 0; i < total; i++)
            {
                var objective = points[i].Objective;

                if (pointsAmount < objective)
                {
                    applyNextObjective(i);
                    _event.ChangeProgressValue((float)(pointsAmount / objective));
                    _event.ChangeProgressText($"{(int)pointsAmount} / {objective}");
                    return;
                }

                pointsAmount -= objective;
            }
            
            var last = points[total - 1];
            applyNextObjective(total);
            _event.ChangeProgressValue(1f);
            _event.ChangeProgressText($"{last.Objective} / {last.Objective}");
            _event.NotifyCompleted();
        }
        
        private void SetPointProgress() =>
            ApplyPointProgress(idx => _event.SetNextObjective(idx));

        private void UpdatePointProgress() =>
            ApplyPointProgress(idx => _event.ChangeNextObjective(idx));

        private void UpdateRoadProgress(double amount)
        {
            if (amount == 0)
            {
                _event.ChangeRoadProgress(0);
                return;
            }

            IReadOnlyList<BattlePassPointGroup> points = _event.Points;
            
            var totalObjectives = points.Sum(p => p.Objective);
            
            if (amount >= totalObjectives)
            {
                _event.ChangeRoadProgress(1f);
                return;
            }

            float segmentSize = (float) 1 / (points.Count - 1);

            for (int i = 0, j = 1; j < points.Count; i++, j++)
            {
                var nextPoint = points[j];

                if (nextPoint.Objective > amount)
                {
                    float amountPerSegmentUnit = segmentSize / nextPoint.Objective;
                    
                    var roadProgress =
                        Mathf.Clamp01(segmentSize * i + amountPerSegmentUnit * (float)amount);
                    
                    _event.ChangeRoadProgress(roadProgress);
                    return;
                }
                
                amount -= nextPoint.Objective;
            }
        }

        private void InitEventTimer()
        {
            if (_eventTimer == null)
                _eventTimer = new SynchronizedCountdownTimer(_event.EventTimeLeft);
            else
                _eventTimer.Reset(_event.EventTimeLeft);

            _eventTimer.TimerStop += OnEventTimerStop;
            _eventTimer.OnTick += OnEventTimerTick;

            _eventTimer.Start();
        }

        private void OnEventTimerTick(float timeLeft)
        {
            _event.OnEventTimerTick(timeLeft);
        }

        private bool EventExpired()
        {
            _logger.Log($"BATTLE PASS EXPIRED {_event.EventTimeLeft <= 0}", DebugStatus.Info);
            return _event.EventTimeLeft <= 0;
        }

        private void OnEventTimerStop()
        {
            _logger.Log($"BATTLE PASS ON TIMER STOP", DebugStatus.Info);
            CheckForUnclaimedEventRewards("TIMER");
            _pointsCell.Change(0);
            Complete?.Invoke(_event);
        }
        
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

        void IGameEventStrategy.UnExecute()
        {
            _logger.Log("BP UNEXECUTE", DebugStatus.Info);
            CheckForUnclaimedEventRewards("UNEXECUTED");
            _pointsCell.Change(0);
            _eventTimer.Pause();
        }
    }
}