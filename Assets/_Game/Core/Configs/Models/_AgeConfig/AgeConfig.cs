using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._EconomyConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories.Timeline;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models._AgeConfig
{
    [CreateAssetMenu(fileName = "AgeConfig", menuName = "Configs/Age")]
    [Serializable]
    public class AgeConfig : ScriptableObject, ILevelConfig
    {
        int ILevelConfig.Level => Level;
        
        public int Id;
        public float Price;
        public float GemsPerAge;
        public EconomyConfig Economy;
        public List<int> WarriorsId;
        public string Name;
        public string AgeIconKey;
        public AssetReference AgeIconAtlas;
        public string AgeIconName;
        [MultiLineProperty(5)] 
        public string Description;
        public string DateRange;
        public AssetReferenceGameObject BasePrefab;
        public int Level;
        public int Order;
        public string FBConfigId;

        public IIconReference GetIconReference() => 
            new IconReference(AgeIconAtlas, AgeIconName);
    }

    public interface IIconReference
    {
        public AssetReference Atlas { get;}
        public string IconName { get; }
    }
}