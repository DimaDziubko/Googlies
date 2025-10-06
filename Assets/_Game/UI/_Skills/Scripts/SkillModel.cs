using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay.Common.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils;
using _Game.Utils._Static;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SkillModel
    {
        public event Action NewMarkChanged;
        
        public event Action<SkillModel> OnLevelUp;
        public event Action<SkillModel> OnLevelChanged;
        public event Action<SkillModel> OnAscensionLevelUp;
        
        
        public ActiveSkill Skill => _skill;
        public float ProgressValue => Mathf.Clamp01((float)_skill.Count / GetUpgradeCount());
        public bool IsReadyForUpgrade => Skill.Count >= GetUpgradeCount();

        private bool _isNew;
        public bool IsNew => _isNew;
        public string Name => _config.Name;
        public string SkillDescription => _config.GetDescription(_skill.Level, IsReadyForUpgrade);
        public Boost[] Boosts => _config.Boosts;
        public bool IsMaxLevel => _skill.Level == _config.MaxLevel;
        public SkillType Type => _config.Type;
        public AudioClip Sfx => _config.Sfx;
        public float Duration => _config.GetDuration(Skill.Level);
        public bool IsCountdownNeeded => _config.IsCountdownNeeded;
        
        public bool IsInterruptible => _config.IsInterruptible;

        private SkillConfig _config;
        
        [ShowInInspector, ReadOnly]
        private ActiveSkill _skill;

        public SkillModel(SkillConfig  config, ActiveSkill skill)
        {
            _config = config;
            _skill = skill;
        }

        public Sprite GetIcon() => _config.GetIcon();

        public int GetUpgradeCount() => _config.GetUpgradeCount(_skill.Level + 1);

        public Color GetBarColor()
        {
            if (IsReadyForUpgrade) return Constants.ColorTheme.THEMED_PROGRESS_BAR_FULL;
            return CardColorMapper.GetColorForType(CardType.Common);
        }
        
        public string GetBarColorName()
        {
            if (IsReadyForUpgrade) return "FullProgressBar";
            return "ProgressBar";
        }
        
        public void SetNew(bool isNew)
        {
            _isNew = isNew;
            NewMarkChanged?.Invoke();
        }

        public void Upgrade()
        {
            int upgradeCount = GetUpgradeCount();
            
            _skill.Remove(upgradeCount);
            _skill.LevelUp();
            
            OnLevelUp?.Invoke(this);
        }

        public void Ascend()
        {
            _skill.ChangeLevel(1);
            _skill.ChangeAmount(0);
            _skill.AscensionLevelUp();
            
            OnLevelChanged?.Invoke(this);
            OnAscensionLevelUp?.Invoke(this);
        }

        public void Equip(int slotId) => _skill.Equip(slotId);
        
        public void UnEquip(int slotId) => _skill.UnEquip();

        public bool IsAscendAvailable()
        {
            return _skill.AscensionLevel < _config.MaxAscensionLevel && _skill.Level == _config.MaxLevel;
        }

        public IEnumerable<Boost> GetBoosts() => _config.Boosts;

        public float GetCurrentPassiveFor(int index)
        {
            if(Boosts.Length <= index) return 1;
            if(_skill.AscensionLevel < 1) return 1;

            int argument = (_skill.AscensionLevel - 1) * _config.MaxLevel + _skill.Level;
            
            return Boosts[index].Linear.GetValue(argument);
        }

        public float GetNextPassiveFor(int index)
        {
            if(Boosts.Length <= index) return 1;

            int argument;
            
            if (_skill.Level == _config.MaxLevel)
            {
                int nextAscensionLevel = (_skill.AscensionLevel + 1);
                
                argument = (nextAscensionLevel - 1) * _config.MaxLevel + 1;
            
                return Boosts[index].Linear.GetValue(argument);
            }
            
            if(_skill.AscensionLevel < 1) return 1;
            argument = (_skill.AscensionLevel - 1) * _config.MaxLevel + _skill.Level + 1;
            return Boosts[index].Linear.GetValue(argument);
        }

        public float GetSkillValue() => 
            _config.GetValue(_skill.Level);

        public float GetSkillLifetime() => 
            _config.GetLifetime();
        public T GetSpecificConfig<T>() where T : SkillConfig => _config as T;
    }
}