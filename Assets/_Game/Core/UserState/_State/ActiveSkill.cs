using System;
using Sirenix.OdinInspector;

namespace _Game.Core.UserState._State
{
    public class ActiveSkill
    {
        public event Action<ActiveSkill> OnLevelUp;
        public event Action<ActiveSkill> OnLevelChanged;
        public event Action<ActiveSkill> OnAscensionLevelUp;
        public event Action<ActiveSkill> SkillEquippedChanged;
        public event Action CountChanged;

        [ShowInInspector, ReadOnly]
        public int Id;
        public int AscensionLevel;
        public int Level;
        public int Count;
        [ShowInInspector, ReadOnly]
        public bool Equipped;
        [ShowInInspector, ReadOnly]
        public int EquippedSlot;

        public void LevelUp()
        {
            Level++;
            OnLevelUp?.Invoke(this);
        }
        
        public void AscensionLevelUp()
        {
            AscensionLevel++;
            OnAscensionLevelUp?.Invoke(this);
        }

        public void Add(int amount)
        {
            Count += amount;
            CountChanged?.Invoke();
        }
        
        public void Remove(int amount)
        {
            Count -= amount;
            CountChanged?.Invoke();
        }
        
        public void ChangeAmount(int amount)
        {
            Count = amount;
            CountChanged?.Invoke();
        }
        
        public void ChangeLevel(int level)
        {
            Level = level;
            OnLevelChanged?.Invoke(this);
        }

        public void Equip(int slot)
        {
            Equipped = true;
            EquippedSlot = slot;
            SkillEquippedChanged?.Invoke(this);
        }

        public void UnEquip()
        {
            Equipped = false;
            EquippedSlot = -1;
            SkillEquippedChanged?.Invoke(this);
        }

        public override string ToString()
        {
            return $"SkillId: {Id} \n" +
                   $"AscendingLevel: {AscensionLevel} \n" +
                   $"SkillLevel: {Level} \n" +
                   $"Count: {Count} \n" +
                   $"Equipped: {Equipped} \n" +
                   $"EquippedSlot: {EquippedSlot}";
        }
    }
}