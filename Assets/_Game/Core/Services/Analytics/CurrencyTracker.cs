using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using DevToDev.Analytics;
using System;

namespace _Game.Core.Services.Analytics
{
    public class CurrencyTracker
    {
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private readonly CurrencyBank _bank;
        private readonly IAnalyticsEventSender _sender;
        private CurrencyCell GemsCell { get; set; }
        private CurrencyCell SkillPotionCell { get; set; }

        public CurrencyTracker(
            CurrencyBank bank,
            IUserContainer userContainer,
            IAnalyticsEventSender sender)
        {
            _sender = sender;
            _bank = bank;
            _userContainer = userContainer;
        }

        public void Initialize()
        {
            GemsCell = _bank.GetCell(CurrencyType.Gems);
            GemsCell.OnAmountAddedFrom += OnGemsEarned;
            GemsCell.OnAmountSpentFrom += OnGemsSpent;
            
            SkillPotionCell = _bank.GetCell(CurrencyType.SkillPotion);
            SkillPotionCell.OnAmountAddedFrom += OnSkillPotionEarned;
            SkillPotionCell.OnAmountSpentFrom += OnSkillPotionEarned;    
        }

        public void Dispose()
        {
            GemsCell.OnAmountAddedFrom -= OnGemsEarned;
            GemsCell.OnAmountSpentFrom -= OnGemsSpent;
            
            SkillPotionCell.OnAmountAddedFrom -= OnSkillPotionEarned;
            SkillPotionCell.OnAmountSpentFrom -= OnSkillPotionEarned;    
        }

        private void OnSkillPotionEarned(double amount, ItemSource source)
        {
            var balance = Math.Min(long.MaxValue, (long)SkillPotionCell.Amount);
            
            DTDAccrualType accrualType;
            if (source == ItemSource.Shop || source == ItemSource.MiniShop) accrualType = DTDAccrualType.Bought;
            else
                accrualType = DTDAccrualType.Earned;

            var parameters = new DTDCustomEventParameters();
            parameters.Add("CurrencyName", "SkillPotion");
            parameters.Add("Amount", amount);
            parameters.Add("Source", source.ToString());
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);

            _sender.CustomEvent("skill_potion_received", parameters);

            _sender.SetUserData("skill_potion_balance", balance);
        }
        
        private void OnGemsSpent(double amount, ItemSource source)
        {
            #region DTD
            var parameters = new DTDCustomEventParameters();
            parameters.Add("CurrencyName", "Gems");
            parameters.Add("Amount", amount);
            parameters.Add("Source", source.ToString());
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);

            _sender.CustomEvent("currencySpent", parameters);

            #endregion
        }

        private void OnGemsEarned(double amount, ItemSource source)
        {
            var balance = Math.Min(long.MaxValue, (long)GemsCell.Amount);
            
            DTDAccrualType accrualType;
            if (source == ItemSource.Shop || source == ItemSource.MiniShop) accrualType = DTDAccrualType.Bought;
            else
                accrualType = DTDAccrualType.Earned;

            var parameters = new DTDCustomEventParameters();
            parameters.Add("CurrencyName", "Gems");
            parameters.Add("Amount", amount);
            parameters.Add("Source", source.ToString());
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);

            _sender.CustomEvent("gems_received", parameters);
            _sender.SetUserData("gems_balance", balance);
        }
    }
}