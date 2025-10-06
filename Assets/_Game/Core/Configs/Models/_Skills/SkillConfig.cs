using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._Skills
{
    public abstract class SkillConfig : ScriptableObject
    {
        public int Id;
        
        public SkillType Type;
        
        public Sprite Icon;
        
        public int MinLevel = 1;
        public int MaxLevel = 20;

        public int DropChance = 100;

        public string Name;
        
        public Boost[] Boosts;
        
        public int MaxAscensionLevel = 3;

        public AudioClip Sfx;
        public bool IsCountdownNeeded = false;
        public bool IsInterruptible = false;
        [FormerlySerializedAs("fbConfigID")] public string FBConfigID;

        public Sprite GetIcon() => Icon;

        public int MaxCount()
        {
            int value = 0;

            for (int i = 1; i <= MaxLevel; i++)
            {
                value += GetUpgradeCount(i);
            }

            return value;
        }

        public int GetAccumulatedCountForLevel(int skillLevel)
        {
            int value = 0;

            for (int i = 1; i <= skillLevel; i++)
            {
                value += GetUpgradeCount(i);
            }

            return value;
        }

        public int GetUpgradeCount(int level)
        {
            if (level < 1 || level > 20)
                return int.MaxValue;

            return (level) / 2;
        }

        public abstract string GetDescription(int skillLevel, bool needShowBoost);

        public abstract float GetValue(int level);

        public virtual float GetLifetime() => 0f;
        public virtual float GetDuration(int level) => 0f;
        
        protected virtual string GetDelta(int currentLevel)
        {
            int nextLevel = currentLevel + 1;
            var delta = GetValue(nextLevel) - GetValue(currentLevel);
            string deltaString = (delta * 100).ToCompactFormat();
            return deltaString;
        }
    }
}