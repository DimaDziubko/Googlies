using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models._Cards
{
    [CreateAssetMenu(fileName = "CardsConfig", menuName = "Configs/CardsConfig")]
    public class CardsConfig : ScriptableObject
    {
        public int Id;
        public List<CardConfig> CardConfigs;
    }
}