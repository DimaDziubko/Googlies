using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._Functions;
using UnityEngine;

namespace _Game.Core.Configs.Models._DifficultyConfig
{
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Configs/Difficulty")]
    [Serializable]
    public class DifficultyConfig : ScriptableObject
    {
        public int Id;
        public Exponential DifficultyCurve;
        public List<float> InitialEvolutionPrices;
        public List<float> DeltaEvolutionPrices;
    }
}