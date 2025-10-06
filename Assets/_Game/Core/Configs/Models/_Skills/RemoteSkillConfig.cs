using System.Collections.Generic;
using _Game.Gameplay._Boosts.Scripts;

namespace _Game.Core.Configs.Models._Skills
{
    public class RemoteSkillConfig
    {
        public int Id;
        public int DropChance;
        public BoostType PassiveType;
        public List<float> PassiveLFC;
        public string FBConfigID;
    }
}