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
                "Warrior_1","Warrior_2",
                "Warrior_3","Warrior_4","Warrior_5",
                "Warrior_6","Warrior_7", "Warrior_8",
                "Warrior_9","Warrior_10", "Warrior_11",
                "Warrior_12","Warrior_13", "Warrior_14",
                "Warrior_15", "Warrior_16","Warrior_17",
                "Warrior_18", "Warrior_19", "Warrior_20",
                "Warrior_21", "Warrior_22", "Warrior_23",
                "Warrior_24", "Warrior_25", "Warrior_26",
                "Warrior_27", "Warrior_28", "Warrior_29",
                "Warrior_30", "Warrior_31", "Warrior_32",
                "Warrior_33", "Warrior_34", "Warrior_35",
                "Warrior_36", "Warrior_37", "Warrior_38",
            };
        }

        private IEnumerable<string> GetEnemyKeys()
        {
            return new List<string>
            {
                "Warrior_1","Warrior_2",
                "Warrior_3","Warrior_4","Warrior_5",
                "Warrior_6","Warrior_7", "Warrior_8",
                "Warrior_9","Warrior_10", "Warrior_11",
                "Warrior_12","Warrior_13", "Warrior_14",
                "Warrior_15", "Warrior_16","Warrior_17",
                "Warrior_18", "Warrior_19", "Warrior_20",
                "Warrior_21", "Warrior_22", "Warrior_23",
                "Warrior_24", "Warrior_25", "Warrior_26",
                "Warrior_27", "Warrior_28", "Warrior_29",
                "Warrior_30", "Warrior_31", "Warrior_32",
                "Warrior_33", "Warrior_34", "Warrior_35",
                "Warrior_36", "Warrior_37", "Warrior_38",
            };
        }

        private IEnumerable<string> GetZombieKeys()
        {
            return new List<string>
            {
                "Warrior_1", "Warrior_4", "Warrior_7",
            };
        }

        private void OnValidate()
        {
            WeaponId = Id;
        }
    }
}