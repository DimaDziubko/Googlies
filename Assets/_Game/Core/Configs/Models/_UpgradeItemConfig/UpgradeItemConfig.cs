using System;
using _Game.Core.Configs.Models._Functions;

namespace _Game.Core.Configs.Models._UpgradeItemConfig
{
    [Serializable]
    public class UpgradeItemConfig
    {
        public int Id;
        public float Price;
        public Exponential PriceExponential;
        public float Value;
        public float ValueStep;
    }
}