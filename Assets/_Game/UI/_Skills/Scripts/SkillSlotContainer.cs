using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlotContainer
    {
        [ShowInInspector, ReadOnly] 
        private readonly Dictionary<int, SkillSlot> _skillSlots;

        private readonly IMyLogger _logger;

        public SkillSlotContainer(IMyLogger logger)
        {
            _logger = logger;
            
            _skillSlots = new Dictionary<int, SkillSlot>(3)
            {
                {1, new SkillSlot(1)},
                {2, new SkillSlot(2)},
                {3, new SkillSlot(3)},
            };
        }

        public void Equip(SkillModel model)
        {
            SkillSlot freeSlot = _skillSlots.Values.FirstOrDefault(x => !x.IsEquipped && !x.IsLocked);

            if (freeSlot == null)
            {
                freeSlot = _skillSlots.Values.First();
            }

            if (freeSlot.IsEquipped) freeSlot.UnEquip(freeSlot.SkillModel);
            
            freeSlot.Equip(model);

            _logger.Log($"EQUIP SLOT: {freeSlot.Id}, WITH SKILL ID: {model.Skill.Id}", DebugStatus.Info);
        }

        public void UnEquip(SkillModel model)
        {
            if (_skillSlots.TryGetValue(model.Skill.EquippedSlot, out SkillSlot slot))
            {
                slot.UnEquip(model);
                
                _logger.Log($"UNEQUIP SLOT: {slot.Id} ", DebugStatus.Info);
            }
        }

        public IEnumerable<SkillSlot> GetSlots() => _skillSlots.Values;

        public SkillSlot GetSlot(int skillViewId)
        {
            return _skillSlots.GetValueOrDefault(skillViewId);
        }
    }
}