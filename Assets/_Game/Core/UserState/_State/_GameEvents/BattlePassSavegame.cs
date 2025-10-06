using System.Collections.Generic;
using _Game.Core._Reward;

namespace _Game.Core.UserState._State._GameEvents
{
    public class BattlePassSavegame
    {
        public int Id;
        
        public List<BattlePassPointRewardSavegame> Points;
        public List<RewardItem> UnclaimedRewards = new();

        public UserProgress Threshold;
        public bool StartSent;
        public bool SpoilerSent;
        public bool ShowSent;

        public CurrencyType PointsCell;

        public void AddUnclaimedReward(RewardItem item)
        {
            UnclaimedRewards.Add(item);
        }
        
        public void ClearUnclaimedRewards()
        {
            UnclaimedRewards.Clear();
        }
        
        public void SetSpoilerSend(bool isSent)
        {
            SpoilerSent = isSent;
        }

        public void SetStartSend(bool isSent)
        {
            StartSent = isSent;
        }
        
        public void SetShowSend(bool isSent)
        {
            ShowSent = isSent;
        }
    }
}