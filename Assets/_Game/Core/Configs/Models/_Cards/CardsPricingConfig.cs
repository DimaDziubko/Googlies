using UnityEngine;

namespace _Game.Core.Configs.Models._Cards
{
    [CreateAssetMenu(fileName = "CardsPricingConfig", menuName = "Configs/CardsPricing")]
    public class CardsPricingConfig : ScriptableObject
    {
        public int Id;
        public int x1CardPrice = 100;
        public int x10CardPrice = 950;
    }
}