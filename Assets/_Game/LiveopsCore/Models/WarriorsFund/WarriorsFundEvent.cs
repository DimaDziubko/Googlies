using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Reward;
using _Game.Core.UserState._State;
using Balancy;
using Balancy.Models;
using Balancy.Models.SmartObjects.Conditions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.LiveopsCore.Models.WarriorsFund
{
    public class WarriorsFundEvent : GameEventBase, IAttentionNotifier
    {
        public event Action ClaimUnclaimedRewardsRequested;
        public event Action RequiredAttention;
        public event Action Purchased;
        public event Action RoadProgressChanged;
        public event Action PointsInitialized;
        public event Action AnalyticsCompleted;

        public EventIAPs IAP { get; private set; }

        public UnnyProduct SegmentIAP => IAP?.ConditionalPrice
            .FirstOrDefault(x => LiveOps.General.CanPassCondition(x.Condition) == PassType.True)?
            .Price.Product;

        public IReadOnlyList<WarriorsFundPointGroup> Points { get; private set; }

        [ShowInInspector, ReadOnly] public List<IRewardItem> UnclaimedRewards { get; private set; }

        [ShowInInspector, ReadOnly] public PurchaseDataState PurchaseData => Save.PurchaseData;

        public bool IsPurchased { get; private set; }
        public float RoadProgress { get; private set; }

        public bool NeedAttention()
        {
            if (Points != null)
            {
                return Points.Any(x => x.IsReady &&
                                       (x.FreePoint.IsRewardReady && !x.FreePoint.IsRewardClaimed)
                                       || (x.FreePoint.IsRewardReady && !x.FreePoint.IsRewardClaimed &&
                                           !x.FreePoint.IsLocked));
            }

            return false;
        }

        [ShowInInspector, ReadOnly]
        public override bool HasUnclaimedRewards() => UnclaimedRewards is { Count: > 0 };

        public void Notify() => RequiredAttention?.Invoke();

        public Sprite TotalRewardIcon { get; private set; }

        public void SetPurchased(bool isPurchased)
        {
            IsPurchased = isPurchased;

            foreach (var point in Points)
            {
                point.SetLocked(!isPurchased);
            }
        }

        public void ChangeRoadProgress(float progress)
        {
            RoadProgress = progress;
            RoadProgressChanged?.Invoke();
        }

        public void ChangePurchased(bool isPurchased)
        {
            if (isPurchased == IsPurchased) return;

            IsPurchased = isPurchased;

            foreach (var point in Points)
            {
                point.ChangeLocked(!isPurchased);
            }

            Purchased?.Invoke();
        }

        public void SetUnclaimedRewards(List<IRewardItem> unclaimedRewards)
        {
            UnclaimedRewards = unclaimedRewards;

            foreach (var model in UnclaimedRewards)
            {
                Save.WarriorsFundSavegame.AddUnclaimedReward(model.Save);
            }
        }

        public void OnUnclaimedRewardsClaimed()
        {
            UnclaimedRewards?.Clear();
            Save.WarriorsFundSavegame?.ClearUnclaimedRewards();
        }

        public void RequestClaimUnclaimedRewards() =>
            ClaimUnclaimedRewardsRequested?.Invoke();

        public void SetPoints(List<WarriorsFundPointGroup> points)
        {
            Points = points;
            PointsInitialized?.Invoke();
        }


        public void InitializePoints()
        {
            if (Points == null) return;

            foreach (var point in Points)
            {
                point.SetNotifier(this);
            }

            RequiredAttention?.Invoke();
        }

        //Analytics
        public bool IsStartSent => Save.WarriorsFundSavegame.StartSent;
        public bool IsShowSent => Save.WarriorsFundSavegame.ShowSent;

        public int AnalyticsLevel
        {
            get
            {
                if (Points == null) return 0;
                return Points.Count(x => x.IsReady);
            }
        }

        public int LastLevelSent => Save.WarriorsFundSavegame.LastLevelSent;
        public void SetStartSent() => Save.WarriorsFundSavegame.SetStartSend(true);
        public void SetShowSent() => Save.WarriorsFundSavegame.SetShowSend(true);
        public void NotifyCompleted() => AnalyticsCompleted?.Invoke();
        public string GetDocumentStr() => $"{Save.Id} {Save.EventName}";
        public void SetLastLevelSent(int level) => Save.WarriorsFundSavegame.SetLastLevelSent(level);

        public class WarriorsFundEventBuilder
        {
            private int _id;
            private string _name;
            private Sprite _icon;
            private GameEventSavegame _save;
            private GameEventShowcaseToken _token;
            private IReadOnlyList<WarriorsFundPointGroup> _points;
            private EventIAPs _iap;
            private List<IRewardItem> _unclaimedRewards;
            private Sprite _totalRewardIcon;

            public WarriorsFundEventBuilder WithId(int saveId)
            {
                _id = saveId;
                return this;
            }

            public WarriorsFundEventBuilder WithSave(GameEventSavegame save)
            {
                _save = save;
                return this;
            }

            public WarriorsFundEventBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public WarriorsFundEventBuilder WithShowcaseToken(GameEventShowcaseToken token)
            {
                _token = token;
                return this;
            }

            public WarriorsFundEventBuilder WithIcon(Sprite icon)
            {
                _icon = icon;
                return this;
            }

            public WarriorsFundEventBuilder WithPoints(List<WarriorsFundPointGroup> points)
            {
                _points = points;
                return this;
            }

            public WarriorsFundEventBuilder WithProducts(EventIAPs iap)
            {
                _iap = iap;
                return this;
            }

            public WarriorsFundEventBuilder WithUnclaimedRewards(List<IRewardItem> unclaimedRewards)
            {
                _unclaimedRewards = unclaimedRewards;
                return this;
            }

            public WarriorsFundEventBuilder WithTotalRewardIcon(Sprite icon)
            {
                _totalRewardIcon = icon;
                return this;
            }

            public WarriorsFundEvent Build()
            {
                var @event = new WarriorsFundEvent()
                {
                    Id = _id,
                    Name = _name,
                    Icon = _icon,
                    Save = _save,
                    ShowcaseToken = _token,
                    Points = _points,
                    IAP = _iap,
                    UnclaimedRewards = _unclaimedRewards,
                    TotalRewardIcon = _totalRewardIcon
                };

                return @event;
            }
        }
    }
}