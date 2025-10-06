using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core._Time;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Core.UserState._State._GameEvents;
using _Game.Gameplay._RewardItemResolver;
using _Game.LiveopsCore._Enums;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using _Game.LiveopsCore.AbstractFactory;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.Utils.BalancyDataExtensions;
using _Game.Utils.Extensions;
using Balancy.Data.SmartObjects;
using Balancy.Models;
using Balancy.Models.SmartObjects;

namespace _Game.LiveopsCore.ConcreteFactory.BattlePassFactory
{
    public class BattlePassEventFactory : GameEventFactoryBase
    {
        public BattlePassEventFactory(IUserContainer userContainer,
            IMyLogger logger,
            IIconConfigRepository iconConfig,
            RewardItemResolver rewardItemResolver,
            IBalancySDKService balancy) :
            base(userContainer, logger, iconConfig, rewardItemResolver, balancy)
        {
        }

        public override GameEventBase CreateModel(EventInfo remote, GameEventSavegame save, bool isNew)
        {
            _logger.Log("BP CREATE MODEL REMOTE", DebugStatus.Info);
            
            if (isNew)
                return CreateNew(remote);
            
            return SynchronizeAndRestore(remote, save);
        }

        private GameEventBase CreateNew(EventInfo remote)
        {
            _logger.Log("BATTLE PASS CREATE NEW", DebugStatus.Info);
    
            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not BattlePassB concrete)
                return null;

            DateTime initialStartDate = concrete.StartDate.DateTimeUTC;
            DateTime now = GlobalTime.UtcNow;

            int durationSec = concrete.Duration;
            int breakSec = concrete.BreakDuration;
            int cycleLengthSec = durationSec + breakSec;
            
            if (!concrete.Repeat && now > initialStartDate.AddSeconds(durationSec))
                return null;
            
            if (now <= initialStartDate.AddSeconds(durationSec))
            {
                DateTime start = initialStartDate;
                DateTime end = start.AddSeconds(durationSec);

                return CreateBattlePassEvent(custom, concrete, start, end, isOnBreak: false);
            }

            double secondsSinceStart = (now - initialStartDate).TotalSeconds;
            int cyclesPassed = (int)(secondsSinceStart / cycleLengthSec);

            DateTime currentCycleStart = initialStartDate.AddSeconds(cyclesPassed * cycleLengthSec);
            DateTime currentCycleEnd = currentCycleStart.AddSeconds(durationSec);
            DateTime breakStart = currentCycleEnd;
            DateTime breakEnd = currentCycleStart.AddSeconds(cycleLengthSec);

            bool isOnBreak = now >= breakStart && now < breakEnd;

            DateTime eventStart = isOnBreak ? breakStart : currentCycleStart;
            DateTime eventEnd = isOnBreak ? breakEnd : currentCycleEnd;

            _logger.Log($"BP START DATE: {eventStart}", DebugStatus.Warning);
            _logger.Log($"BP END DATE: {eventEnd}", DebugStatus.Info);
            _logger.Log($"BP IS ON BREAK: {isOnBreak}", DebugStatus.Info);

            return CreateBattlePassEvent(custom, concrete, eventStart, eventEnd, isOnBreak);
        }

        private GameEventBase CreateBattlePassEvent(
            CustomGameEvent custom,
            BattlePassB concrete,
            DateTime startDate,
            DateTime endDate,
            bool isOnBreak)
        {
            var save = new GameEventSavegame
            {
                Id = custom.IntUnnyId,
                EventName = custom.Name.ToString(),
                StartDateUtc = startDate,
                EndDateUtc = endDate,
                IsOnBreak = isOnBreak,
                EventType = concrete.GameEventType.ToLocal(),
                SortOrder = custom.SortOrder,
                PanelType = custom.GameEventPanel.ToLocal(),
                SlotType = custom.SlotTypeB.ToLocal(),
                Offers = new List<int>(1) { concrete.EventIAP.IntUnnyId },

                ShowcaseSave = new GameEventShowcaseSavegamne
                {
                    ShowcaseCondition = custom.ShowcaseCondition.ToLocal(),
                    ShowOrder = custom.ShowOrder
                },

                BattlePassSavegame = new BattlePassSavegame
                {
                    Id = concrete.IntUnnyId,
                    Points = ResolveListPoint(concrete.Points),
                    UnclaimedRewards = new(),
                    Threshold = concrete.Threshold.ToLocal(),
                    PointsCell = concrete.PointsCell.ToLocal()
                },

                PurchaseData = new PurchaseDataState()
            };

            return BuildEvent(save, true);
        }
        
        private List<BattlePassPointRewardSavegame> ResolveListPoint(BattlePassPointB[] concretePoints)
        {
            List<BattlePassPointRewardSavegame> points = new List<BattlePassPointRewardSavegame>(concretePoints.Length);
            foreach (var point in concretePoints)
            {
                var localPoint = new BattlePassPointRewardSavegame()
                {
                    Objective = point.Objective,
                    FreePoint = ResolvePoint(point.FreeReward),
                    PremiumPoint = ResolvePoint(point.PremiumReward),
                };
                
                points.Add(localPoint);
            }
            
            return points;
        }

        private RewardItem ResolvePoint(ItemWithAmount itemWithAmount)
        {
            return new RewardItem()
            {
                Id = itemWithAmount.Item.IntUnnyId,
                IsRewardClaimed = false,
                Amount = itemWithAmount.Count
            };
        }

        private GameEventBase SynchronizeAndRestore(EventInfo remote, GameEventSavegame save)
        {
            _logger.Log("BATTLE PASS SYNC AND RESTORE", DebugStatus.Info);
            
            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not BattlePassB concrete)
                return null;
            
            save.Id = custom.IntUnnyId;
            save.EventType = concrete.GameEventType.ToLocal();
            save.SortOrder = custom.SortOrder;
            save.PanelType = custom.GameEventPanel.ToLocal();
            save.SlotType = custom.SlotTypeB.ToLocal();
            save.Offers = new List<int>(1) {concrete.EventIAP.IntUnnyId};
            save.ShowcaseSave ??= new GameEventShowcaseSavegamne();
            save.ShowcaseSave.ShowcaseCondition = custom.ShowcaseCondition.ToLocal();
            save.ShowcaseSave.ShowOrder = custom.ShowOrder;
            
            save.BattlePassSavegame ??= new BattlePassSavegame();

            save.BattlePassSavegame.PointsCell = concrete.PointsCell.ToLocal();
            
            if (save.BattlePassSavegame.Points == null || save.BattlePassSavegame.Points.Count == 0)
            {
                save.BattlePassSavegame.Points = ResolveListPoint(concrete.Points);
            }

            if (save.PurchaseData == null)
            {
                save.PurchaseData = new PurchaseDataState();
                _logger.Log("BATTLE PASS PURCHASE DATA IS NULL", DebugStatus.Info);
            }
            else
            {
                _logger.Log($"BATTLE PASS PURCHASE DATA COUNT {save.PurchaseData.BoudhtIAPs.Count}", DebugStatus.Info);
            }
            
            DateTime endTime = save.IsOnBreak
                ? save.StartDateUtc.AddSeconds(concrete.BreakDuration)
                : save.StartDateUtc.AddSeconds(concrete.Duration);
            
            save.EndDateUtc = endTime;
            
            _logger.Log($"BP START TIME {save.StartDateUtc}", DebugStatus.Info);
            _logger.Log($"BP END TIME {endTime}", DebugStatus.Info);
            _logger.Log($"BP IS ON BREAK: {save.IsOnBreak}", DebugStatus.Info);
            
            return BuildEvent(save);
        }
        
        private GameEventBase BuildEvent(GameEventSavegame save, bool isNew = false)
        {
            EventIAPs[] iaps = _balancy.GetEventIAPs();

            int eventIapId = 0;
            EventIAPs selectedEventIap = null;
            
            if (save.Offers is { Count: > 0 })
            {
                eventIapId = save.Offers[0];
            }
            
            foreach (var iap in iaps)
            {
                if (iap.IntUnnyId == eventIapId)
                {
                    selectedEventIap = iap;
                }
            }
            
            GameEventShowcaseToken showcaseToken = new GameEventShowcaseToken(save.ShowcaseSave);
            
            bool isShown = !isNew && IsShown(showcaseToken.ShowcaseCondition, save);
            
            showcaseToken.SetShown(isShown);

            CurrencyDropSettings currencyDropSettings = GetCurrencyDropSettings(save.Id);

            var builder = new BattlePassEvent.BattlePassBuilder()
                .WithId(save.Id)
                .WithIcon(_iconConfig.GetEventIconFor(save.EventName))
                .WithSave(save)
                .WithName(save.EventName)
                .WithCurrencyCellType(save.BattlePassSavegame.PointsCell)
                .WithPoints(_rewardItemResolver.Resolve(save.BattlePassSavegame.Points))
                .WithShowcaseToken(showcaseToken)
                .WithProducts(selectedEventIap)
                .WithUnclaimedRewards(_rewardItemResolver.Resolve(save.BattlePassSavegame.UnclaimedRewards))
                .WithCurrencyDropSettings(currencyDropSettings);

            return builder.Build();
        }

        private CurrencyDropSettings GetCurrencyDropSettings(int saveId)
        {
            IEnumerable<CustomGameEvent> allEvents = _balancy.GetAllEvents();
            
            foreach (var @event in allEvents)
            {
                if (@event.IntUnnyId == saveId && @event.ConcreteEvent is BattlePassB battlePassEvent)
                {
                    return battlePassEvent.CurrencyDropSettings.ToLocal();
                }
            }
            
            return CurrencyDropSettings.CreateDefault();
        }

        private bool IsShown(ShowcaseCondition condition, GameEventSavegame save)
        {
            switch (condition)
            {
                case ShowcaseCondition.Never:
                    return true;
                case ShowcaseCondition.OnStart:
                    return false;
                 case ShowcaseCondition.OnUpdate:
                     return save.ShowcaseSave.IsShown;
                 case ShowcaseCondition.New:
                     return save.ShowcaseSave.IsShown;
                 default: 
                     return save.ShowcaseSave.IsShown;
            }
        }
        
        public override GameEventBase CreateModel(GameEventSavegame save) => 
            BuildEvent(save);

        public override GameEventBase TryCreateWithPendingRewards(GameEventSavegame save)
        {
            if (save.BattlePassSavegame?.UnclaimedRewards?.Count == 0)
                return null;
            
            return BuildEvent(save);
        }
    }
}