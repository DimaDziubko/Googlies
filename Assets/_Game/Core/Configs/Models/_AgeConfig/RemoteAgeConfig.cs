using System.Collections.Generic;
using _Game.Core.Configs.Models._EconomyConfig;

namespace _Game.Core.Configs.Models._AgeConfig
{
    public class RemoteAgeConfig
    {
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<int> WarriorsId;
        public int Level;
        public int Order;
        public string FBConfigId;
    }
}