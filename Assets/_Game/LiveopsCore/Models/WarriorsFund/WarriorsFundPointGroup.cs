using System;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.BattlePass;
using UnityEngine;

namespace _Game.LiveopsCore.Models.WarriorsFund
{
    public class WarriorsFundPointGroup
    {
        public event Action OnChangeReady;
        public UserProgress Objective {get; private set;}
        public RewardPoint FreePoint { get; private set; }
        public RewardPoint PremiumPoint { get; private set; }
        
        public Sprite Icon => IsReady ? _readyIcon :_notReadyIcon;
        public bool IsReady { get; private set; }
        
        private readonly Sprite _notReadyIcon;
        private readonly Sprite _readyIcon;

        public WarriorsFundPointGroup(
            UserProgress objective,
            Sprite notReadyIcon,
            Sprite readyIcon,
            RewardPoint freePoint,
            RewardPoint premiumPoint)
        {
            Objective = objective;
            
            _notReadyIcon = notReadyIcon;
            _readyIcon = readyIcon;
            
            FreePoint = freePoint;
            PremiumPoint = premiumPoint;
        }

        public void SetReady(bool isReady) => ApplyReady(isReady, false);
        public void ChangeReady(bool isReady) => ApplyReady(isReady, true);

        private void ApplyReady(bool isReady, bool notify)
        {
            if(IsReady == isReady) return;
            IsReady = isReady;
            FreePoint.ChangeReady(isReady);
            PremiumPoint.ChangeReady(isReady);
            if(notify) OnChangeReady?.Invoke();
        }

        public void SetLocked(bool isLocked) => PremiumPoint.SetLocked(isLocked);
        public void ChangeLocked(bool isLocked) => PremiumPoint.ChangeLocked(isLocked);

        public void SetNotifier(IAttentionNotifier notifier)
        {
            FreePoint.SetNotifier(notifier);
            PremiumPoint.SetNotifier(notifier);
        }
    }
}