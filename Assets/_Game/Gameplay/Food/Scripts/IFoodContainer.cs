using System;
using UnityEngine;

namespace _Game.Gameplay.Food.Scripts
{
    public interface IFoodContainer
    {
        public event Action OnStateChanged;
        public event Action<int> OnAmountChanged;
        public event Action<int> OnAmountAdded;
        public event Action<int> OnAmountSpent;
        public event Action<float> OnProgressChanged;

        public int Amount { get; }
        public float Progress { get; }
        public Sprite FoodIcon { get; }
        float ProductionSpeed { get;}

        bool Add(int range);
        bool Spend(int range);
        void Change(int amount);
        void Reset();
        bool IsEnough(float range);
        void SetProgress(float progress);
        void Clear();
    }
}