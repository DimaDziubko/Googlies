using System;

namespace _Game.Core.UserState._State._GameEvents
{
    public class ClassicOfferSavegame
    {
        public int TakenCount;
        public RewardItemCollection RewardsCollection;
        public int Discount;
        public int PurchaseLimit;
        public UserProgress Threshold;
        public float DurationMinutes;
        public DateTime CurrentStageStart;
        public string ConditionalProductId;

        public bool StartSent;


        public void OfferTaken()
        {
            ++TakenCount;
        }
        public void SetStartSend(bool isSent)
        {
            StartSent = isSent;
        }
    }
}
