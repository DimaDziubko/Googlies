using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core._Time;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardItemResolver;
using _Game.LiveopsCore._Enums;
using _Game.LiveopsCore.AbstractFactory;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.Utils.Extensions;
using Balancy.Data.SmartObjects;
using Balancy.Models;
using Balancy.Models.SmartObjects;
using Cysharp.Threading.Tasks;

namespace _Game.LiveopsCore.ConcreteFactory.WarriorsFundFactory
{
    public class WarriorsFundEventFactory : GameEventFactoryBase
    {
        public WarriorsFundEventFactory(IUserContainer userContainer,
            IMyLogger logger,
            IIconConfigRepository iconConfig,
            RewardItemResolver rewardItemResolver,
            IBalancySDKService balancy) :
            base(userContainer, logger, iconConfig, rewardItemResolver, balancy)
        {
        }

        public override GameEventBase CreateModel(EventInfo remote, GameEventSavegame save, bool isNew)
        {
            _logger.Log("WARRIORS_FUND MODEL REMOTE", DebugStatus.Info);

            if (isNew)
                return CreateNew(remote);

            return SynchronizeAndRestore(remote, save);
        }

        private GameEventBase CreateNew(EventInfo remote)
        {
            _logger.Log("WARRIORS FUND CREATE NEW", DebugStatus.Info);

            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not WarriorsFundB concrete)
                return null;
            
            DateTime eventStart = GlobalTime.UtcNow;
            DateTime eventEnd = GlobalTime.UtcNow.AddSeconds(100000); //Mock
            
            return CreateWarriorsFundEvent(custom, concrete, eventStart, eventEnd, false);
        }

        private GameEventBase CreateWarriorsFundEvent(
            CustomGameEvent custom,
            WarriorsFundB concrete,
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

                PurchaseData = new PurchaseDataState(),
                
                WarriorsFundSavegame = new WarriorsFundSavegame()
                {
                    Id = concrete.IntUnnyId,
                    Points = ResolveListItem(concrete.Items),
                    UnclaimedRewards = new(),
                    Threshold = concrete.Threshold.ToLocal(),
                }
            };

            return BuildEvent(save, true);
        }

        private List<WarriorsFundPointRewardSavegame> ResolveListItem(WarriorsFundItemB[] concretePoints)
        {
            List<WarriorsFundPointRewardSavegame> points = new List<WarriorsFundPointRewardSavegame>(concretePoints.Length);
            foreach (var point in concretePoints)
            {
                var localPoint = new WarriorsFundPointRewardSavegame()
                {
                    Objective = point.Objective.ToLocal(),
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
            _logger.Log("WARRIORS_FUND SYNC AND RESTORE", DebugStatus.Info);

            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not WarriorsFundB concrete)
                return null;

            save.Id = custom.IntUnnyId;
            save.EventName = custom.Name.ToString();
            save.EventType = concrete.GameEventType.ToLocal();
            save.SortOrder = custom.SortOrder;
            save.PanelType = custom.GameEventPanel.ToLocal();
            save.SlotType = custom.SlotTypeB.ToLocal();
            save.Offers = new List<int>(1) { concrete.EventIAP.IntUnnyId};
            save.ShowcaseSave ??= new GameEventShowcaseSavegamne();
            save.ShowcaseSave.ShowcaseCondition = custom.ShowcaseCondition.ToLocal();
            save.ShowcaseSave.ShowOrder = custom.ShowOrder;

            save.WarriorsFundSavegame ??= new WarriorsFundSavegame();
            
            if (save.WarriorsFundSavegame.Points == null || save.WarriorsFundSavegame.Points.Count == 0)
            {
                save.WarriorsFundSavegame.Points = ResolveListItem(concrete.Items);
            }
            
            save.WarriorsFundSavegame.UnclaimedRewards ??= new List<RewardItem>();

            if (save.PurchaseData == null)
            {
                save.PurchaseData = new PurchaseDataState();
                _logger.Log("WARRIORS_FUND PURCHASE DATA IS NULL", DebugStatus.Info);
            }
            else
            {
                _logger.Log($"WARRIORS_FUND PURCHASE DATA COUNT {save.PurchaseData.BoudhtIAPs.Count}", DebugStatus.Info);
            }
            
            _logger.Log($"WARRIORS_FUND START TIME {save.StartDateUtc}", DebugStatus.Info);
            _logger.Log($"WARRIORS_FUND IS ON BREAK: {save.IsOnBreak}", DebugStatus.Info);

            return BuildEvent(save);
        }

        private GameEventBase BuildEvent(GameEventSavegame save, bool isNew = false)
        {
            EventIAPs[] iaps = _balancy.GetEventIAPs();
            int eventIapId = save.Offers?.Count > 0 ? save.Offers[0] : 0;
            EventIAPs selectedEventIap = Array.Find(iaps, x => x.IntUnnyId == eventIapId);

            GameEventShowcaseToken showcaseToken = new GameEventShowcaseToken(save.ShowcaseSave);
            bool isShown = !isNew && IsShown(showcaseToken.ShowcaseCondition, save);
            showcaseToken.SetShown(isShown);

            var builder = new WarriorsFundEvent.WarriorsFundEventBuilder()
                .WithId(save.Id)
                .WithIcon(_iconConfig.GetEventIconFor(save.EventName))
                .WithSave(save)
                .WithName(save.EventName)
                .WithPoints(null)
                .WithShowcaseToken(showcaseToken)
                .WithProducts(selectedEventIap)
                .WithUnclaimedRewards(_rewardItemResolver.Resolve(save.WarriorsFundSavegame.UnclaimedRewards))
                .WithTotalRewardIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.Gems));

            var evt =  builder.Build();
            
            _ = PopulatePointsAsync(evt, save);
            
            return evt;
        }
        
        private async UniTask PopulatePointsAsync(WarriorsFundEvent evt, GameEventSavegame save)
        {
            List<WarriorsFundPointGroup> points = await _rewardItemResolver.ResolveAsync(save.WarriorsFundSavegame.Points);
            
            evt.SetPoints(points);
            evt.InitializePoints();
            
            _logger.Log($"[WarriorsFund] Points and rewards populated asynchronously", DebugStatus.Info);
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
            if (save.WarriorsFundSavegame?.UnclaimedRewards?.Count == 0)
                return null;

            return BuildEvent(save);
        }
    }
}