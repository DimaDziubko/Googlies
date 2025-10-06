using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Reward;
using _Game.Core._Time;
using _Game.Core.UserState._State;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using Balancy;
using Balancy.Models;
using Balancy.Models.SmartObjects.Conditions;
using Sirenix.OdinInspector;
using SRF;
using UnityEngine;

namespace _Game.LiveopsCore.Models.BattlePass
{
    public class BattlePassEvent : GameEventBase, IAttentionNotifier
    {
        public event Action ClaimUnclaimedRewardsRequested;
        public event Action<float> EventTimerTick;
        public event Action RoadProgressChanged;
        public event Action Purchased;
        public event Action ObjectiveChanged;
        public event Action ProgressChanged;
        public event Action ProgressTextChanged;
        public event Action RequiredAttention;
        public event Action AnalyticsCompleted;
        

        public float EventTimeLeft => (float)(Save.EndDateUtc - GlobalTime.UtcNow).TotalSeconds;
        
        public void NotifyCompleted() => AnalyticsCompleted?.Invoke();

        public void OnEventTimerTick(float timeLeft) => EventTimerTick?.Invoke(timeLeft);

        public IReadOnlyList<BattlePassPointGroup> Points { get; private set; }
        
        [ShowInInspector, ReadOnly]
        public List<IRewardItem> UnclaimedRewards { get; private set; }
        
        public override bool HasUnclaimedRewards() => UnclaimedRewards is { Count: > 0 };
        
        public float RoadProgress { get; private set; }

        public EventIAPs IAP { get; private set; }
        public UnnyProduct SegmentIAP => IAP?.ConditionalPrice
            .FirstOrDefault(x => LiveOps.General.CanPassCondition(x.Condition) == PassType.True)?
            .Price.Product; 

        [ShowInInspector, ReadOnly]
        public PurchaseDataState PurchaseData => Save.PurchaseData;
        
        public void ChangeRoadProgress(float roadProgress)
        {
            RoadProgress = roadProgress;
            RoadProgressChanged?.Invoke();
        }

        public bool IsPurchased { get; private set; }

        public void RequestClaimUnclaimedRewards() => 
            ClaimUnclaimedRewardsRequested?.Invoke();

        public CurrencyDropSettings CurrencyDropSettings { get; private set; }

        public CurrencyType CurrencyCellType { get; private set; }

        public int NextObjective { get; private set; }

        public float Progress { get; private set; }

        public string ProgressText { get; private set; }

        public bool ShowedSent => Save.BattlePassSavegame.ShowSent;

        public bool IsStartSent => Save.BattlePassSavegame.StartSent;

        public void SetPurchased(bool isPurchased)
        { 
            IsPurchased = isPurchased;
            
            foreach (var point in Points)
            {
                point.SetLocked(!isPurchased);
            }
        }

        public void ChangePurchased(bool isPurchased)
        { 
            if(isPurchased == IsPurchased) return;
            
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
                Save.BattlePassSavegame.AddUnclaimedReward(model.Save);
            }
        }

        public void OnUnclaimedRewardsClaimed()
        {
            UnclaimedRewards?.Clear();
            Save.BattlePassSavegame?.ClearUnclaimedRewards();
        }

        public void ChangeProgressValue(float progress)
        {
            if (Progress.Approx(progress)) return;
            Progress = progress;
            ProgressChanged?.Invoke();
        }

        public void ChangeProgressText(string progressText)
        {
            if(ProgressText == progressText) return;
            ProgressText = progressText;
            ProgressTextChanged?.Invoke();
        }

        public void SetNextObjective(int nextObjective) => 
            NextObjective = nextObjective;

        public void ChangeNextObjective(int nextObjective)
        {
            if(NextObjective == nextObjective) return;
            NextObjective = nextObjective;
            ObjectiveChanged?.Invoke();
        }

        public bool NeedAttention() =>
            Points.Any(x => x.IsReady &&
                            (x.FreePoint.IsRewardReady && !x.FreePoint.IsRewardClaimed)
                            || (x.FreePoint.IsRewardReady && !x.FreePoint.IsRewardClaimed && !x.FreePoint.IsLocked));

        public void Notify() => 
            RequiredAttention?.Invoke();

        //Analytics

        public string GetFormatedTimeForAnalytics()
        {
            string formatted = $"{Save.StartDateUtc.Date} - {Save.EndDateUtc.Date}";
            return formatted;
        }
        public void SetShowedSent() => Save.BattlePassSavegame.SetShowSend(true);

        public void SetStartSent() => Save.BattlePassSavegame.SetStartSend(true);

        public string GetDocumentStr() => $"{Save.BattlePassSavegame.Id} {Name}";

        public class BattlePassBuilder
        {
            private int _id;
            private string _name;
            private Sprite _icon;
            private GameEventSavegame _save;
            private GameEventShowcaseToken _token;
            private CurrencyType _currencyCellType;
            private IReadOnlyList<BattlePassPointGroup> _points;
            private EventIAPs _iap;
            private List<IRewardItem> _unclaimedRewards;
            private CurrencyDropSettings _currencyDropSettings;

            public BattlePassBuilder WithId(int saveId)
            {
                _id = saveId;
                return this;
            }

            public BattlePassBuilder WithSave(GameEventSavegame save)
            {
                _save = save;
                return this;
            }

            public BattlePassBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public BattlePassBuilder WithShowcaseToken(GameEventShowcaseToken token)
            {
                _token = token;
                return this;
            }

            public BattlePassBuilder WithIcon(Sprite icon)
            {
                _icon = icon;
                return this;
            }
            
            public BattlePassBuilder WithCurrencyCellType(CurrencyType type)
            {
                _currencyCellType = type;
                return this;
            }

            public BattlePassBuilder WithPoints(List<BattlePassPointGroup> points)
            {
                _points = points;
                return this;
            }

            public BattlePassBuilder WithProducts(EventIAPs iap)
            {
                _iap = iap;
                return this;
            }

            public BattlePassBuilder  WithCurrencyDropSettings(CurrencyDropSettings currencyDropSettings)
            {
                _currencyDropSettings = currencyDropSettings;
                return this;
            }

            public BattlePassBuilder WithUnclaimedRewards(List<IRewardItem> unclaimedRewards)
            {
                _unclaimedRewards = unclaimedRewards;
                return this;
            }

            public BattlePassEvent Build()
            {
                var @event = new BattlePassEvent()
                {
                    Id = _id,
                    Name = _name,
                    Icon = _icon,
                    Save = _save,
                    ShowcaseToken = _token,
                    CurrencyCellType = _currencyCellType,
                    Points = _points,
                    IAP = _iap,
                    UnclaimedRewards = _unclaimedRewards,
                    CurrencyDropSettings = _currencyDropSettings
                };

                foreach (var point in @event.Points)
                {
                    point.SetNotifier(@event);
                }
                
                return @event;
            }
        }
    }
}