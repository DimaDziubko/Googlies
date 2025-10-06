using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models._Dungeons
{
    [CreateAssetMenu(fileName = "DungeonsConfig", menuName = "Configs/Dungeons")]
    [Serializable]
    public class DungeonsConfig : ScriptableObject
    {
        public int Id;
        public List<DungeonConfig> Dungeons;
    }
}