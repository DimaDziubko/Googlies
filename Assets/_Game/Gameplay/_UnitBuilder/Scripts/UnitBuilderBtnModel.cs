using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderBtnModel
    {
        public UnitType Type { get;}
        public int FoodPrice { get;}
        public bool IsUnlocked { get;}
        public Sprite FoodIcon { get;}
        public Sprite Icon { get;}
        
        public UnitBuilderBtnModel(
            UnitType type,
            Sprite foodIcon,
            Sprite icon,
            int foodPrice,
            bool isUnlocked)
        {
            Type = type;
            FoodIcon = foodIcon;
            FoodPrice = foodPrice;
            IsUnlocked = isUnlocked;
            Icon = icon;
        }
    }
}