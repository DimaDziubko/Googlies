using System;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlot
    {
        public event Action Equipped;
        public event Action UnEquipped;
        
        public int Id => _id;
        
        [ShowInInspector, ReadOnly]
        private int _id;
        
        [ShowInInspector, ReadOnly]
        public bool IsEquipped => SkillModel != null;
        
        [ShowInInspector, ReadOnly]
        public bool IsLocked { get; private set; }
        
        [ShowInInspector, ReadOnly]
        private SkillModel _skillModel;
        
        public SkillModel SkillModel => _skillModel;
        public int TimelineThreshold { get; private set; }

        public SkillSlot(int id)
        {
            _id = id;
        }
        
        public void Equip(SkillModel model)
        {
            if (_skillModel != null) 
                UnEquip(_skillModel);
            
            _skillModel = model;
            
            _skillModel.Equip(_id);
            
            Equipped?.Invoke();
        }

        public void UnEquip(SkillModel model)
        {
            _skillModel?.UnEquip(_id);
            _skillModel = null;
            
            UnEquipped?.Invoke();
        }

        public void SetLocked(bool isLocked)
        {
            IsLocked = isLocked;
        }

        public void SetTimelineThreshold(int timelineThreshold)
        {
            TimelineThreshold = timelineThreshold;
        }
    }
}