using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.IAP;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPopupPresenter
    {
        public event Action RoadProgressChanged;
        public event Action Purchased;
        
        private WarriorsFundEvent _event;

        private readonly WarriorsFundPurchasePopupPresenter _warriorsFundPurchasePopupPresenter;
        
        public WarriorsFundPopupPresenter(
            WarriorsFundEvent @event, 
            IAPProvider iAPProvider,
            IMyLogger logger)
        {
            _event = @event;
            _warriorsFundPurchasePopupPresenter = new WarriorsFundPurchasePopupPresenter(@event, iAPProvider, logger);
        }

        public void Initialize()
        {
            _event.RoadProgressChanged += OnRoadProgressChanged;
            _event.Purchased += OnPurchased;
        }

        public void Dispose()
        {
            _event.RoadProgressChanged -= OnRoadProgressChanged;
            _event.Purchased -= OnPurchased;
        }

        private void OnPurchased()
        {
            Purchased?.Invoke();
        }

        private void OnRoadProgressChanged()
        {
            RoadProgressChanged?.Invoke();
        }

        public void SetModel(WarriorsFundEvent @event) => _event = @event;
        public WarriorsFundPurchasePopupPresenter GetPurchasePresenter() => _warriorsFundPurchasePopupPresenter;
        public IReadOnlyCollection<WarriorsFundPointGroup> GetPoints() => _event.Points;
        public bool IsPurchased() => _event.IsPurchased;
        public Sprite GetTotalRewardIcon() => _event.TotalRewardIcon;
        public string GetTotalRewardAmount() => 
            _event.Points.Sum(p => p.FreePoint.Amount + p.PremiumPoint.Amount).ToCompactFormat(20000);
        public float GetRoadProgressValue() => _event.RoadProgress;
    }
}