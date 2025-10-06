using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._AgeConfig
{
    [CreateAssetMenu(fileName = "GeneralAgesConfig", menuName = "Configs/Ages")]
    public class GeneralAgesConfig : ScriptableObject
    {
        public List<AgeConfig> AgeConfigs;
        
        [Button("Validate Unique IDs")]
        public void ValidateUniqueIds()
        {
            HashSet<int> uniqueIds = new HashSet<int>();

            foreach (var age in AgeConfigs)
            {
                if (!uniqueIds.Add(age.Id))
                {
                    Debug.LogError($"Duplicate ID found: {age.Id}");
                }
            }
        }
    }
}