using System;

namespace _Game.LiveopsCore.Models.BattlePass
{
    public class BattlePassPointGroup
    {
        public event Action OnChangeReady;
        public int Objective { get; private set; }
        
        public RewardPoint FreePoint { get; private set; }
        public RewardPoint PremiumPoint { get; private set; }
        public bool IsReady => _isReady;
        
        private bool _isReady;

        public BattlePassPointGroup(
            int objective, 
            RewardPoint freePoints, 
            RewardPoint preferredPoints)
        {
            Objective = objective;
            FreePoint = freePoints;
            PremiumPoint = preferredPoints;
        }

        public void SetReady(bool isReady)
        {
            if(isReady == _isReady) return;
            _isReady = isReady;
            FreePoint.SetReady(isReady);
            PremiumPoint.SetReady(isReady);
        }
        
        public void ChangeReady(bool isReady)
        {
            if(isReady == _isReady) return;
            _isReady = isReady;
            FreePoint.ChangeReady(isReady);
            PremiumPoint.ChangeReady(isReady);
            OnChangeReady?.Invoke();
        }

        public void SetLocked(bool isLocked)
        {
            PremiumPoint.SetLocked(isLocked);
        }
        
        public void ChangeLocked(bool isLocked)
        {
            PremiumPoint.ChangeLocked(isLocked);
        }

        public void SetNotifier(IAttentionNotifier notifier)
        {
            FreePoint.SetNotifier(notifier);
            PremiumPoint.SetNotifier(notifier);
        }
    }
}