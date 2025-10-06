using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Reward;
using _Game.Core._Time;
using _Game.Core.UserState._State;
using Balancy;
using Balancy.Models;
using Balancy.Models.SmartObjects.Conditions;
using UnityEngine;

namespace _Game.LiveopsCore.Models.ClassicOffer
{
    public class ClassicOfferEvent : GameEventBase, IAttentionNotifier
    {
        public event Action<float> OnEventTimerTick;
        public event Action OnIconChange;
        public event Action OnOfferPurchased;

        public event Action OnSetHiden;
        public event Action OnShowed;

        public IReadOnlyList<RewardItemModel> Rewards { get; private set; }
        public int PurchaseLimit { get; private set; }
        public int Discount { get; private set; }
        public UserProgress Threshold => Save.ClassicOfferSavegame.Threshold;

        public float EventTimeLeft => (float)(Save.EndDateUtc - GlobalTime.UtcNow).TotalSeconds;

        public void InvokeEventTimerTick(float timeLeftSeconds) => OnEventTimerTick?.Invoke(timeLeftSeconds);
        //public Sprite GetIcon(Race currentRace) =>
        public int GetAvalivableCount => PurchaseLimit - Save.ClassicOfferSavegame.TakenCount;
        public int GetTakenCount => Save.ClassicOfferSavegame.TakenCount;

        public bool IsHiden => Save.IsHiden;
        public float CycleDurationMinutes => Save.ClassicOfferSavegame.DurationMinutes;
        public DateTime CurrentStageStart { get; private set; }
        public int RewardListID => Save.ClassicOfferSavegame.RewardsCollection.Id;

        public EventIAPs IAP { get; private set; }
        public PurchaseDataState PurchaseData => Save.PurchaseData;

        public void PurchaseOffer()
        {
            Save.ClassicOfferSavegame.OfferTaken();
            OnOfferPurchased?.Invoke();
        }


        public void RequestHideEvent()
        {
            OnSetHiden?.Invoke();
        }

        public void ChangeCycle(DateTime startTime, DateTime endTime)
        {
            Save.SetStartTime(startTime);
            Save.SetEndTime(endTime);

            Save.ClassicOfferSavegame.TakenCount = 0;
            Save.SetHiden(false);

            //CycleChanged?.Invoke();
        }
        public void NotifyShowed() => OnShowed?.Invoke();
        public void SetStartSend(bool isSent) => Save.ClassicOfferSavegame.SetStartSend(isSent);
        //public void SetSpoilerSend(bool isSent) => Save.ClassicOfferSavegame.SetSpoilerSend(isSent);
        public bool IsStartSent() => Save.ClassicOfferSavegame.StartSent;

        public void Notify()
        {

        }

        public bool IsPurchased { get; private set; }

        public UnnyProduct SegmentIAP()
        {
            if (IAP == null) return null;

            // Try to find the product by the saved conditional product ID
            UnnyProduct savedProduct = null;
            if (!string.IsNullOrEmpty(Save.ClassicOfferSavegame.ConditionalProductId))
            {
                savedProduct = IAP.AllPrices
                    .FirstOrDefault(x => x.Product.ProductId == Save.ClassicOfferSavegame.ConditionalProductId)?
                    .Product;
            }

            // if not found, try to find the first product that passes the condition
            return savedProduct ?? IAP.ConditionalPrice
                .FirstOrDefault(x => LiveOps.General.CanPassCondition(x.Condition) == PassType.True)?
                .Price.Product;
        }

        internal void SetPurchased(bool isPurchased)
        {
            IsPurchased = isPurchased;
        }

        internal void ChangePurchased(bool isPurchased)
        {
            if (isPurchased == IsPurchased) return;

            IsPurchased = isPurchased;

            //Purchased?.Invoke();
        }

        internal void NotifyCompleted()
        {

        }

        //public bool IsSpoilerShowedSent() => Save.ClassicOfferSavegame.SpoilerSent;

        public class ClassicOfferBuilder
        {
            int _id = 0;
            GameEventSavegame _save;
            IReadOnlyList<RewardItemModel> _rewards;
            private int _purchaseLimit;
            private int _discount;
            private string _name;
            private Sprite _icon;
            private DateTime _currentStageStart;
            private GameEventShowcaseToken _token;
            private EventIAPs _iap;

            public ClassicOfferBuilder WithId(int id)
            {
                _id = id;
                return this;
            }

            public ClassicOfferBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public ClassicOfferBuilder WithIcon(Sprite sprite)
            {
                _icon = sprite;
                return this;
            }

            public ClassicOfferBuilder WithSave(GameEventSavegame save)
            {
                _save = save;
                return this;
            }

            public ClassicOfferBuilder WithRewards(IReadOnlyList<RewardItemModel> rewards)
            {
                _rewards = rewards;
                return this;
            }

            public ClassicOfferBuilder WithPurchaseLimit(int count)
            {
                _purchaseLimit = count;
                return this;
            }

            public ClassicOfferBuilder WithDiscount(int value)
            {
                _discount = value;
                return this;
            }

            public ClassicOfferBuilder WithProducts(EventIAPs iap)
            {
                _iap = iap;
                return this;
            }

            public ClassicOfferBuilder WithCurrentStageStart(DateTime startTime)
            {
                _currentStageStart = startTime;
                return this;
            }

            //New
            public ClassicOfferBuilder WithShowcaseToken(GameEventShowcaseToken token)
            {
                _token = token;
                return this;
            }

            public ClassicOfferEvent Build()
            {
                var @event = new ClassicOfferEvent()
                {
                    Id = _id,
                    Name = _name,
                    Icon = _icon,
                    Save = _save,
                    Rewards = _rewards,
                    PurchaseLimit = _purchaseLimit,
                    Discount = _discount,
                    CurrentStageStart = _currentStageStart,
                    ShowcaseToken = _token,
                    IAP = _iap

                };

                return @event;
            }
        }

    }
}
