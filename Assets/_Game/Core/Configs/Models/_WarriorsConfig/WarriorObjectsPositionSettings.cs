using System;
using UnityEngine;

namespace _Game.Core.Configs.Models._WarriorsConfig
{
    [Serializable]
    public struct WarriorObjectsPositionSettings
    {
        public ObjectPositionSettings HealthBarSettings;
        public ObjectPositionSettings SkillEffectParentSettings;
        public ObjectPositionSettings DamageTextPointSettings;
    }

    [Serializable]
    public struct ObjectPositionSettings
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
        
        public void Apply(Transform target)
        {
            if (target == null) return;

            target.localPosition = Position;
            target.localEulerAngles = Rotation;
            target.localScale = Scale;
        }
    }
}