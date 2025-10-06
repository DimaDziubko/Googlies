using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Core.Configs.Repositories.Timeline;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models._BattleConfig
{
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "Configs/Battle")]
    [Serializable]
    public class BattleConfig : ScriptableObject, IConfigWithId, ILevelConfig
    {
        int IConfigWithId.Id => Id;
        public int Id;
        public BattleScenario Scenario;
        public List<int> WarriorsId;
        public string EnvironmentKey;
        public float EnemyBaseHealth;
        public string AmbienceKey;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
        public AssetReferenceGameObject BasePrefab;
        int ILevelConfig.Level => Level;
        public int Level;
        public string FBConfigId;
    }
    
    public class RemoteBattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<int> WarriorsId;
        public float EnemyBaseHealth;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
        public string FBConfigId;
    }
}