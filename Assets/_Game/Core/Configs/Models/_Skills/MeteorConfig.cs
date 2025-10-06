using System;
using _Game.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._Skills
{
    [Serializable]
    public class MeteorConfig
    {
        [SerializeField]
        [FloatRangeSlider(-0.5f, 1)]
        private FloatRange ScaleRange = new(0.5f, 1);

        [SerializeField]
        [FloatRangeSlider(3f, 6f)]
        private FloatRange SpeedRange = new(3f, 6f);

        [SerializeField]
        [FloatRangeSlider(0f, 500f)]
        private FloatRange RotationSpeedRange = new(0f, 500f);

        private FloatRange Clockwise = new(-1f, 1f);
        
        public float DamageRadius = 0.3f;
        
        public string PrefabKey = "Meteor";
        public string ExplosionKey = "MeteorExplosion";
        public AudioClip ExplodeSound;
        
        public float ShakeMagnitude = 0.1f;
        public float ShakeDuration = 0.5f;

        public Vector3 GetRandomScale()
        {
            var scale = ScaleRange.RandomValueInRange;
            return new Vector3(scale, scale, scale);
        }
        
        public float GetRandomSpeed() => 
            SpeedRange.RandomValueInRange;

        public bool GetRotationDirection() => Clockwise.RandomValueInRange < 0;

        public float GetRandomRotationSpeed() => 
            RotationSpeedRange.RandomValueInRange;

    }
}