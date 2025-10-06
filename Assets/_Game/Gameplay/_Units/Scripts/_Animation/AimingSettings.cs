using System;
using _Game.Utils;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts._Animation
{
    [Serializable]
    public class AimingSettings
    {
        public float MaxAimDistance = 2;
        
        [FloatRangeSlider(-60f, 60f)] 
        public FloatRange AngleRange = new(-45f, 45f);

        [Range(0.01f, 0.3f)] 
        public float AimSmoothTime = 0.1f;
    }
}