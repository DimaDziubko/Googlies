using System;
using System.Collections.Generic;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using UnityEngine;
using _Game.Core._Logger;
using _Game.Core.Services.IAP;
using _Game.LiveopsCore.Models.BattlePass;
using Sirenix.OdinInspector;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassPopupPresenter
    {
        public event Action<float> EventTimerTick;
        public event Action RoadProgressChanged;
        public event Action Purchased;
        public event Action ObjectiveChanged;
        public event Action ProgressChanged;
        public event Action ProgressTextChanged;

        private readonly IIconConfigRepository _iconConfig;

        [ShowInInspector, ReadOnly]
        private BattlePassEvent _event;
        
        private readonly BattlePassPurchasePopupPresenter _battlePassPurchasePopupPresenter;
        
        public BattlePassPopupPresenter(
            BattlePassEvent @event,
            IIconConfigRepository iconConfig,
            IAPProvider iAPProvider, 
            IMyLogger logger)
        {
            _event = @event;
            _iconConfig = iconConfig;
            _battlePassPurchasePopupPresenter = new BattlePassPurchasePopupPresenter(@event, iAPProvider, logger);
        }

        public void SetModel(BattlePassEvent @event) => 
            _event = @event;

        public void Initialize()
        {
            _event.EventTimerTick += OnEventTimerTick;
            _event.RoadProgressChanged += OnRoadProgressChanged;
            _event.Purchased += OnPurchased;
            
            _event.ObjectiveChanged += OnObjectiveChanged;
            _event.ProgressChanged += OnProgressChanged;
            _event.ProgressTextChanged += OnProgressTextChanged;
        }

        private void OnProgressTextChanged() => 
            ProgressTextChanged?.Invoke();

        private void OnProgressChanged() => 
            ProgressChanged?.Invoke();

        private void OnObjectiveChanged() => 
            ObjectiveChanged?.Invoke();

        private void OnPurchased() => 
            Purchased?.Invoke();

        public void Dispose()
        {
            _event.EventTimerTick -= OnEventTimerTick;
            _event.RoadProgressChanged -= OnRoadProgressChanged;
            _event.Purchased -= OnPurchased;
            
            _event.ObjectiveChanged -= OnObjectiveChanged;
            _event.ProgressChanged -= OnProgressChanged;
            _event.ProgressTextChanged -= OnProgressTextChanged;
        }

        private void OnRoadProgressChanged() 
            => RoadProgressChanged?.Invoke();

        private void OnEventTimerTick(float timeLeft) => 
            EventTimerTick?.Invoke(timeLeft);

        public float GetTimeLeft() => _event.EventTimeLeft;

        public Sprite GetPointIcon() => 
            _iconConfig.GetCurrencyIconFor(_event.CurrencyCellType);

        public int GetPointInlineIndex() => 
            _iconConfig.GetPointInlineIndex(_event.CurrencyCellType);

        public IReadOnlyList<BattlePassPointGroup> GetPoints() => 
            _event.Points;

        public float GetProgressValue() => 
            _event.Progress;

        public string GetProgressTitle() => 
            _event.ProgressText;

        public string GetNextPointNumberText() => 
            _event.NextObjective.ToString(); 
        
        public float GetRoadProgressValue() => 
            _event.RoadProgress;

        public BattlePassPurchasePopupPresenter GetPurchasePresenter() => 
            _battlePassPurchasePopupPresenter;

        public bool IsPurchased() => _event.IsPurchased;
    }
}