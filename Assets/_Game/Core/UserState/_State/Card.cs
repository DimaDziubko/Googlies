using System;

namespace _Game.Core.UserState._State
{
    public class Card
    {
        public event Action<Card> OnLevelUp;
        public event Action CountChanged;
        
        public int Id;
        public int Level;
        public int Count;
        public bool Equipped;
        public int EquippedSlot;
        
        public void LevelUp()
        {
            Level++;
            OnLevelUp?.Invoke(this);
        }

        public void AddCard(int amount)
        {
            Count += amount;
            CountChanged?.Invoke();
        }
        
        public void RemoveCard(int amount)
        {
            Count -= amount;
            CountChanged?.Invoke();
        }
    }
}