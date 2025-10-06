using System;
using System.Collections.Generic;
using System.Linq;
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
using _Game.LiveopsCore.AbstractFactory;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.Utils.Extensions;
using Balancy;
using Balancy.Data.SmartObjects;
using Balancy.Models;
using Balancy.Models.SmartObjects;
using Balancy.Models.SmartObjects.Conditions;

namespace _Game.LiveopsCore.Models.BattlePass
{
    public class ClassicOfferEventFactory : GameEventFactoryBase
    {

        public ClassicOfferEventFactory(
            IUserContainer userContainer,
            IMyLogger logger,
            IIconConfigRepository iconConfig,
            RewardItemResolver resolver,
            IBalancySDKService balancy
            )
            : base(userContainer, logger, iconConfig, resolver, balancy)
        {

        }

        public override GameEventBase CreateModel(EventInfo remote, GameEventSavegame save, bool isNew)
        {
            if (isNew)
            {
                return CreateNew(remote);
            }

            return SynchronizeAndRestore(remote, save);
        }

        private GameEventBase CreateNew(EventInfo remote)
        {
            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not ClassicOfferB concrete)
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

                return CreateEvent(custom, concrete, start, end, isOnBreak: false);
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

            return CreateEvent(custom, concrete, eventStart, eventEnd, isOnBreak);
        }

        private GameEventBase CreateEvent(
                    CustomGameEvent custom,
                    ClassicOfferB concrete,
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
                Offers = new List<int>(1) { concrete.IAP.IntUnnyId },

                ShowcaseSave = new GameEventShowcaseSavegamne
                {
                    ShowcaseCondition = custom.ShowcaseCondition.ToLocal(),
                    ShowOrder = custom.ShowOrder
                },
                ClassicOfferSavegame = new ClassicOfferSavegame
                {
                    TakenCount = 0,
                    RewardsCollection = CreateRewardsFromConditionalList(concrete.ConditionalRewards),
                    ConditionalProductId = ConditionalProductId(concrete.IAP),
                    Discount = concrete.Discount,
                    PurchaseLimit = concrete.PurchaseLimit,
                    Threshold = concrete.Threshold.ToLocal(),
                    CurrentStageStart = concrete.StartDate.DateTimeUTC,
                },

                PurchaseData = new PurchaseDataState()
            };

            return BuildEvent(save, true);
        }
        private GameEventBase SynchronizeAndRestore(EventInfo remote, GameEventSavegame save)
        {
            _logger.Log("BATTLE PASS SYNC AND RESTORE", DebugStatus.Info);

            if (remote.GameEvent is not CustomGameEvent custom || custom.ConcreteEvent is not ClassicOfferB concrete)
                return null;

            save.Id = custom.IntUnnyId;
            save.EventType = concrete.GameEventType.ToLocal();
            save.SortOrder = custom.SortOrder;
            save.PanelType = custom.GameEventPanel.ToLocal();
            save.SlotType = custom.SlotTypeB.ToLocal();
            save.Offers = new List<int>(1) { concrete.IAP.IntUnnyId };
            save.ShowcaseSave ??= new GameEventShowcaseSavegamne();
            save.ShowcaseSave.ShowcaseCondition = custom.ShowcaseCondition.ToLocal();
            save.ShowcaseSave.ShowOrder = custom.ShowOrder;

            save.ClassicOfferSavegame ??= new ClassicOfferSavegame { TakenCount = 0 };
            save.PurchaseData ??= new PurchaseDataState { };

            if (save.EventName != remote.GameEvent.Name.ToString())
                save.EventName = remote.GameEvent.Name.ToString();

            save.ClassicOfferSavegame.CurrentStageStart = concrete.StartDate.DateTimeUTC;

            save.ClassicOfferSavegame.RewardsCollection ??= CreateRewardsFromConditionalList(concrete.ConditionalRewards);

            save.ClassicOfferSavegame.ConditionalProductId = ConditionalProductId(concrete.IAP);

            if (save.PurchaseData == null)
            {
                save.PurchaseData = new PurchaseDataState();
                _logger.Log("[Classic Offer] PURCHASE DATA IS NULL", DebugStatus.Info);
            }
            else
            {
                _logger.Log($"[Classic Offer] PURCHASE DATA COUNT {save.PurchaseData.BoudhtIAPs.Count}", DebugStatus.Info);
            }

            DateTime endTime = save.IsOnBreak
                ? save.StartDateUtc.AddSeconds(concrete.BreakDuration)
                : save.StartDateUtc.AddSeconds(concrete.Duration);

            save.EndDateUtc = endTime;

            _logger.Log($"[Classic Offer]  START TIME {save.StartDateUtc}", DebugStatus.Info);
            _logger.Log($"[Classic Offer]  END TIME {endTime}", DebugStatus.Info);
            _logger.Log($"[Classic Offer]  IS ON BREAK: {save.IsOnBreak}", DebugStatus.Info);

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

            var builder = new ClassicOfferEvent.ClassicOfferBuilder()
                .WithId(save.Id)
                .WithIcon(_iconConfig.GetEventIconFor(save.EventName)) //"Classic Offer"
                .WithSave(save)
                .WithRewards(
                    _rewardItemResolver.ResolveRewardsCollection(save.ClassicOfferSavegame.RewardsCollection)
                        .RewardItems
                )
                .WithName(save.EventName)
                .WithPurchaseLimit(save.ClassicOfferSavegame.PurchaseLimit)
                .WithCurrentStageStart(save.ClassicOfferSavegame.CurrentStageStart)
                .WithDiscount(save.ClassicOfferSavegame.Discount)
                .WithShowcaseToken(showcaseToken)
                .WithProducts(selectedEventIap);

            return builder.Build();
        }

        private bool IsShown(ShowcaseCondition condition, GameEventSavegame save)
        {
            return condition switch
            {
                ShowcaseCondition.Never => true,
                ShowcaseCondition.OnStart => false,
                ShowcaseCondition.OnUpdate => save.ShowcaseSave.IsShown,
                ShowcaseCondition.New => save.ShowcaseSave.IsShown,
                _ => save.ShowcaseSave.IsShown,
            };
        }

        private RewardItemCollection CreateRewardsFromConditionalList(ConditionalRewardList[] remoteRewards)
        {
            RewardItemCollection collection = new RewardItemCollection();

            var tasksRewards = new List<RewardItem>();

            var selected =
                remoteRewards.FirstOrDefault(x => LiveOps.General.CanPassCondition(x.Condition) == PassType.True);

            if (selected is not null && selected.Rewards is { Length: > 0 })
            {
                foreach (var remoteReward in selected.Rewards)
                {
                    tasksRewards.Add(CreateNewRewardItem(remoteReward));
                }

                collection.Id = selected.Id;
            }
            else
            {
                tasksRewards.Add(RewardItem.CreateDefault());
            }

            collection.CycleRewards = tasksRewards;

            return collection;
        }

        private string ConditionalProductId(EventIAPs IAP)
        {
            return IAP?.ConditionalPrice
                .FirstOrDefault(x => LiveOps.General.CanPassCondition(x.Condition) == PassType.True)?
                .Price.Product.ProductId;
        }

        private RewardItem CreateNewRewardItem(ItemWithAmount remote) =>
            new() { Id = remote.Item.IntUnnyId, Amount = remote.Count, IsRewardClaimed = false };

        public override GameEventBase CreateModel(GameEventSavegame save) => BuildEvent(save);

        public override GameEventBase TryCreateWithPendingRewards(GameEventSavegame save)
        {
            //No Unclaimed Rewards
            return null;
        }
    }
}
