using System.Collections.Generic;

namespace _Game.Gameplay._Units.Scripts
{
    public enum DebuffType
    {
        Ice,
        Shockwave,
        Shine,
        Strength
    }

    public interface IDebuff
    {
        DebuffType DebuffType { get; }
        void Assign(UnitBase unitBase);
        void Remove();
    }

    public abstract class DebuffMediator
    {
        public abstract void AddOrReplace(IDebuff debuff);
        public abstract void Remove(DebuffType debuffType);
        public abstract void Dispose();
        
    }
    
    public class DebuffUnitMediator : DebuffMediator
    {
        private readonly UnitBase _unit;
        private readonly Dictionary<DebuffType, IDebuff> _activeDebuffs;

        public DebuffUnitMediator(UnitBase unit)
        {
            _unit = unit;
            _activeDebuffs = new Dictionary<DebuffType, IDebuff>();
        }

        public override void AddOrReplace(IDebuff debuff)
        {
            if (_activeDebuffs.TryGetValue(debuff.DebuffType, out var existingDebuff))
            {
                existingDebuff?.Remove();
            }
            _activeDebuffs[debuff.DebuffType] = debuff;
            debuff.Assign(_unit);
        }

        public override void Remove(DebuffType debuffType)
        {
            if (_activeDebuffs.TryGetValue(debuffType, out var existingDebuff))
            {
                existingDebuff?.Remove();
            }
        }

        public override void Dispose()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (var debuff in _activeDebuffs.Values)
            {
                debuff.Remove();
            }
            _activeDebuffs.Clear();
        }
    }

}