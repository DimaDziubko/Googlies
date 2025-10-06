using System.Collections;
using System.Collections.Generic;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Navigation.Timeline;
using _Game.Core.UserState._State;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonResultPopupPresenter
    {
        private readonly IDungeonModel _model;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly IIconConfigRepository _iconConfig;

        public DungeonResultPopupPresenter(
            TemporaryCurrencyBank temporaryBank,
            IIconConfigRepository iconConfig,
            IDungeonModel model)
        {
            _iconConfig = iconConfig;
            _model = model;
            _temporaryBank = temporaryBank;
        }
        
        public string GetDungeonName() =>_model.Name;
        public Sprite GetRewardIcon() => _model.RewardIcon;
        public string GetRewardAmount() => _model.GetRewardAmount.ToCompactFormat();
        public void OnCollect()
        {
            _model.SpendKey();

            _temporaryBank.Add(_model.Reward);

            if (_model.CurrentLevel == _model.Dungeon.Level)
            {
                _model.LevelUp();
            } 
            
            _model.MoveToNextLevel();
        }

        public IEnumerable<CurrencyCell> GetAdditionalRewards() => 
            _temporaryBank;

        public Sprite GetIconFor(CurrencyType cellType) => 
            _iconConfig.GetCurrencyIconFor(cellType);
    }
}