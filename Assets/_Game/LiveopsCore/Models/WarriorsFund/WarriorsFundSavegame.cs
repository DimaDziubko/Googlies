using System.Collections.Generic;
using _Game.Core._Reward;
using _Game.Core.UserState._State;

namespace _Game.LiveopsCore.Models.WarriorsFund
{
    public class WarriorsFundSavegame
    {
        public int Id;
        
        public List<WarriorsFundPointRewardSavegame> Points;
        public List<RewardItem> UnclaimedRewards = new();

        public UserProgress Threshold;
        public bool StartSent;
        public bool SpoilerSent;
        public bool ShowSent;
        public int LastLevelSent;

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

        public void SetLastLevelSent(int level)
        {
            LastLevelSent = level;
        }
    }
}