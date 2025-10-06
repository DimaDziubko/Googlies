using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._WeaponConfig
{
    [CreateAssetMenu(fileName = "GeneralWeaponConfig", menuName = "Configs/Weapons")]
    [Serializable]
    public class GeneralWeaponConfig : ScriptableObject
    {
        public int Id;
        public List<WeaponConfig> Weapons;
        
        [Button("Validate Unique IDs")]
        public void ValidateUniqueIds()
        {
            HashSet<int> uniqueIds = new HashSet<int>();

            foreach (var weapon in Weapons)
            {
                if (!uniqueIds.Add(weapon.Id))
                {
                    Debug.LogError($"Duplicate ID found: {weapon.Id}");
                }
            }
        }
    }
}