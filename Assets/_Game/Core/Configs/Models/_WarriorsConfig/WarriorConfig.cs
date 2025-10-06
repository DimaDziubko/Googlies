using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils;
using Pathfinding.RVO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models._WarriorsConfig
{
    public enum Skin
    {
        None,
        
        Ally,
        Hostile,
        Zombie,
        
        GreenGhost,
        BlueGhost,
        MagentaGhost
    }
    
    [CreateAssetMenu(fileName = "WarriorConfig", menuName = "Configs/Warrior")]
    [Serializable]
    public class WarriorConfig : ScriptableObject, IConfigWithId
    {
        int IConfigWithId.Id => Id;

        public int Id;
        public UnitType Type;
        public UnitLayerType UnitLayerType;
        public float Health;
        public float Speed;
        public string Name;
        public AssetReference PlayerIconAtlas;
        public AssetReference EnemyIconAtlas;

        [ValueDropdown("GetPlayerIconNames")]
        public string PlayerIconName;

        [ValueDropdown("GetEnemyIconNames")]
        public string EnemyIconName;

        [ValueDropdown("GetPlayerKeys")]
        public string PlayerKey;

        [ValueDropdown("GetEnemyKeys")]
        public string EnemyKey;

        [ValueDropdown("GetZombieKeys")]
        public string ZombieKey;


        public float Price;
        public int FoodPrice;
        public int CoinsPerKill;
        public float PlayerHealthMultiplier;
        public float EnemyHealthMultiplier;
        public float AttackPerSecond;
        public float AttackDistance;
        public float DefaultAttackDistance = 0.14f;
        public bool IsPushable = true;

        public WarriorObjectsPositionSettings WarriorPositionSettings;
        public WarriorObjectsPositionSettings ZombiePositionSettings;


        [ReadOnly]
        public int WeaponId;

        public string FBConfigId;

        public WarriorObjectsPositionSettings GetPositionSettings(Skin skin)
        {
            return skin switch
            {
                Skin.Zombie => ZombiePositionSettings,
                _ => WarriorPositionSettings
            };
        }
        
        public string GetPrefabKeyFor(Skin skin)
        {
            return skin switch
            {
                Skin.None => PlayerKey,
                Skin.Ally => PlayerKey,
                Skin.Hostile => EnemyKey,
                Skin.Zombie => ZombieKey != "" ? ZombieKey : PlayerKey,
                _ => PlayerKey
            };
        }

        public AssetReference GetIconAtlasFor(Skin skin)
        {
            return skin switch
            {
                Skin.None => PlayerIconAtlas,
                Skin.Ally => PlayerIconAtlas,
                Skin.Hostile => EnemyIconAtlas,
                Skin.Zombie => EnemyIconAtlas,
                _ => PlayerIconAtlas
            };
        }

        public string GetIconNameFor(Skin skin)
        {
            return skin switch
            {
                Skin.None => PlayerIconName,
                Skin.Ally => PlayerIconName,
                Skin.Hostile => EnemyIconName,
                Skin.Zombie => EnemyIconName,
                _ => PlayerIconName
            };
        }
        
        public int GetAggroLayer(Faction faction)
        {
            return faction switch
            {
                Faction.Enemy => Constants.Layer.ENEMY_AGGRO,
                Faction.Player => Constants.Layer.PLAYER_AGGRO,
                _ => Constants.Layer.ENEMY_AGGRO
            };
        }

        public int GetUnitLayer(Faction faction)
        {
            return (faction, UnitLayerType) switch
            {
                (Faction.Player, UnitLayerType.Melee) => Constants.Layer.MELEE_PLAYER,
                (Faction.Player, UnitLayerType.Range) => Constants.Layer.RANGE_PLAYER,
                (Faction.Enemy, UnitLayerType.Melee) => Constants.Layer.MELEE_ENEMY,
                (Faction.Enemy, UnitLayerType.Range) => Constants.Layer.RANGE_ENEMY,
                _ => Constants.Layer.MELEE_PLAYER
            };
        }

        public RVOLayer GetRVOLayer(Faction faction)
        {
            return (faction, UnitLayerType) switch
            {
                (Faction.Player, UnitLayerType.Melee) => Constants.Layer.RVO_MELEE_PLAYER,
                (Faction.Player, UnitLayerType.Range) => Constants.Layer.RVO_RANGE_PLAYER,
                (Faction.Enemy, UnitLayerType.Melee) => Constants.Layer.RVO_MELEE_ENEMY,
                (Faction.Enemy, UnitLayerType.Range) => Constants.Layer.RVO_RANGE_ENEMY,
                _ => Constants.Layer.RVO_MELEE_PLAYER
            };
        }
        
        public int GetAttackLayer(Faction faction)
        {
            return faction switch
            {
                Faction.Enemy => Constants.Layer.ENEMY_ATTACK,
                Faction.Player => Constants.Layer.PLAYER_ATTACK,
                _ => Constants.Layer.ENEMY_ATTACK
            };
        }

        public IIconReference GetIconReferenceFor(Skin skin)
        {
            return skin switch
            {
                Skin.None => new IconReference(PlayerIconAtlas, PlayerIconName),
                Skin.Ally => new IconReference(PlayerIconAtlas, PlayerIconName),
                Skin.Hostile => new IconReference(EnemyIconAtlas, EnemyIconName),
                Skin.Zombie => new IconReference(EnemyIconAtlas, EnemyIconName),
                _ => new IconReference(PlayerIconAtlas, PlayerIconName),
            };
        }
        
        private IEnumerable<string> GetPlayerIconNames() => 
            new List<string> { "0", "1", "2" };

        private IEnumerable<string> GetEnemyIconNames() => 
            new List<string> { "0", "1", "2" };

        private IEnumerable<string> GetPlayerKeys()
        {
            return new List<string>
            {
                "Unit_3", "Unit_4", "Unit_5",
                "Unit_6", "Unit_7", "Unit_8",
                "Unit_9", "Unit_10", "Unit_11",
                "Unit_12", "Unit_13", "Unit_14",
                "Unit_15", "Unit_16", "Unit_17",
                "Unit_18", "Unit_19", "Unit_20",
                "Unit_21", "Unit_22", "Unit_23",
                "Unit_24", "Unit_25", "Unit_26",
                "Unit_27", "Unit_28", "Unit_29",
                "Unit_30", "Unit_31", "Unit_32",
                //Spine
                "Unit_0", "Unit_1", "Unit_2",
                "Unit_33", "Unit_34", "Unit_35",
                "Unit_36", "Unit_37", "Unit_38",
                "Unit_39", "Unit_40", "Unit_41",
                "Unit_42", "Unit_43", "Unit_44",
                "Unit_45", "Unit_46", "Unit_47",
                "Unit_48", "Unit_49", "Unit_50",
                "Unit_51", "Unit_52", "Unit_53",
                "Unit_100", "Warrior_4","Warrior_5",
                "Warrior_7", "Warrior_8", "Warrior_9",  
                "Warrior_10", "Warrior_11", "Warrior_12",
                "Warrior_13", "Warrior_14", "Warrior_15",
            };
        }

        private IEnumerable<string> GetEnemyKeys()
        {
            return new List<string>
            { 
                "Enemy_3", "Enemy_4", "Enemy_5",
                "Enemy_6", "Enemy_7", "Enemy_8",
                "Enemy_9", "Enemy_10", "Enemy_11",
                "Enemy_12", "Enemy_13", "Enemy_14",
                "Enemy_15", "Enemy_16", "Enemy_17",
                "Enemy_18", "Enemy_19", "Enemy_20",
                "Enemy_21", "Enemy_22", "Enemy_23",
                "Enemy_24", "Enemy_25", "Enemy_26",
                "Enemy_27", "Enemy_28", "Enemy_29",
                "Enemy_30", "Enemy_31", "Enemy_32",
                //Spine
                "Unit_0", "Unit_1",  "Unit_2",
                "Unit_33", "Unit_34", "Unit_35",
                "Unit_36", "Unit_37", "Unit_38",
                "Unit_39", "Unit_40", "Unit_41",
                "Unit_42", "Unit_43", "Unit_44",
                "Unit_45", "Unit_46", "Unit_47",
                "Unit_48", "Unit_49", "Unit_50",
                "Unit_51", "Unit_52", "Unit_53",
                "Unit_100", "Warrior_4", "Warrior_7",
                "Warrior_8", "Warrior_5", "Warrior_9",
                "Warrior_10", "Warrior_11", "Warrior_12",
                "Warrior_13", "Warrior_14", "Warrior_15",
            };
        }
        
        private IEnumerable<string> GetZombieKeys()
        {
            return new List<string>
            {
                "Zombie_3", 
                "Zombie_6", 
                "Zombie_9", 
                "Zombie_18", 
                "Zombie_21", 
                "Zombie_24", 
                "Zombie_27", 
                //Spine
                "Unit_0",
                "Unit_36",
                "Unit_39",
                "Unit_42",
                "Unit_45",
            };
        }
        
        private void OnValidate()
        {
            WeaponId = Id;
        }
    }
}