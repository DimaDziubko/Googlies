using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;

namespace _Game.UI._Skills.Scripts
{
    public class SkillPopupPresenter
    {
        public event Action StateChanged;
        
        private SkillModel _model;

        private readonly SkillSlotContainer _slotContainer;
        private readonly IMyLogger _logger;

        public SkillPopupPresenter(
            SkillModel model,
            SkillSlotContainer slotContainer,
            IMyLogger logger)
        {
            _model = model;
            _slotContainer = slotContainer;
            _logger = logger;
        }

        public void Initialize()
        {
            _model.Skill.OnLevelUp += OnLevelUp;
            _model.Skill.CountChanged += OnCountChanged;
            _model.Skill.SkillEquippedChanged += OnEquippedChanged;
            _model.Skill.OnAscensionLevelUp += OnAscendLevelChanged;
        }

        public void Dispose()
        {
            _model.Skill.OnLevelUp -= OnLevelUp;
            _model.Skill.CountChanged -= OnCountChanged;
            _model.Skill.SkillEquippedChanged -= OnEquippedChanged;
            _model.Skill.OnAscensionLevelUp -= OnAscendLevelChanged;
        }
        
        public SkillModel Model => _model;
        public bool IsReadyForUpgrade => _model.IsReadyForUpgrade;
        public int Level => _model.Skill.Level;
        public bool IsEquipped => _model.Skill.Equipped;
        public bool IsAscendAvailable => _model.IsAscendAvailable();

        public string GetName() => _model.Name;
        public string GetDescription() => _model.SkillDescription;

        public void Upgrade()
        {
            if(!IsReadyForUpgrade) return;
            _model.Upgrade();
        }
        
        private void OnCountChanged() => StateChanged?.Invoke();
        private void OnLevelUp(ActiveSkill skill) => StateChanged?.Invoke();
        private void OnEquippedChanged(ActiveSkill skill) => StateChanged?.Invoke();
        private void OnAscendLevelChanged(ActiveSkill skill) => StateChanged?.Invoke();
        
        public bool HasPassiveBoosts() => Model.Skill.AscensionLevel > 0;

        public void RequestEquip()
        {
            _slotContainer.Equip(_model);
            _logger.Log("PRESENTER REQUEST EQUIP", DebugStatus.Info);
        }

        public void RequestUnEquip()
        {
            _slotContainer.UnEquip(_model);
            _logger.Log("PRESENTER REQUEST UNEQUIP", DebugStatus.Info);
        }

        public IEnumerable<Boost> GetBoosts() => _model.GetBoosts();

        public float GetCurrentPassiveFor(int index)
        {
            return _model.GetCurrentPassiveFor(index);
        }

        public float GetNextPassiveFor(int index)
        {
            return _model.GetNextPassiveFor(index);
        }

        public Boost GetBoost(int index)
        {
            if(_model.Boosts.Length <= index) return new Boost();
            return _model.Boosts[index];
        }

        public void SetModel(SkillModel skillModel) => _model = skillModel;
    }
}