using _Game.Utils;
using _Game.Utils.Extensions;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "GhostsSkillConfig", menuName = "Configs/Skill/Ghosts")]
    public class GhostsSkillConfig : SkillConfig
    {
        public float InitialHealthPercentage = 0.5f;
        public float HealthPercentagePerLevel = 0.13f;
        public float GhostUnitSpeed = 0.4f;
        public string GhostPrefabKey = "Ghost";
        public float AttackPerSecond = 1f;

        public override string GetDescription(int level, bool needShowBoost)
        {
            if(needShowBoost)
                return $"Spawn three fast-moving ghosts, each with {(GetValue(level) * 100).ToCompactFormat(100)}% <style=Green>(+{GetDelta(level)})%</style> of Starting Unit Health";
            
            return $"Spawn three fast-moving ghosts, each with {(GetValue(level) * 100).ToCompactFormat(100)}% of Starting Unit Health";
        }
        
        public override float GetValue(int level) => 
            InitialHealthPercentage + HealthPercentagePerLevel * (level - 1);

        public int GetUnitLayer() => Constants.Layer.SOLO_PLAYER;
        public int GetAggroLayer() => Constants.Layer.PLAYER_AGGRO; 
        public int GetAttackLayer() => Constants.Layer.PLAYER_ATTACK;
        public RVOLayer GetRVOLayer() => Constants.Layer.RVO_SOLO_PLAYER;
    }
}