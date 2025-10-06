using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._BattleConfig
{
    [CreateAssetMenu(fileName = "GeneralBattlesConfig", menuName = "Configs/Battles")]
    public class GeneralBattlesConfig : ScriptableObject
    {
        public List<BattleConfig> BattleConfigs;
        
        [Button("Validate Unique IDs")]
        public void ValidateUniqueIds()
        {
            HashSet<int> uniqueIds = new HashSet<int>();

            foreach (var battle in BattleConfigs)
            {
                if (!uniqueIds.Add(battle.Id))
                {
                    Debug.LogError($"Duplicate ID found: {battle.Id}");
                }
            }
        }
    }
}