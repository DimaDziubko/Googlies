using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._Functions
{
    [Serializable]
    public class Exponential
    {
        [SerializeField, OnValueChanged("UpdateCoefficients")]
        private int _id;
        public int Id 
        {
            get => _id;
            set
            {
                _id = value;
                UpdateCoefficients();
            }
        }
        
        public float A;
        public float B;
        public float C;
        public float D;

        private void UpdateCoefficients()
        {
            (A, B, C, D) = GetValueForId(Id);
        }
        
        public float GetValue(int level)
        {
            float cost = A * Mathf.Exp(B + C * level) + D;
            return cost;
        }
        
        public void SetValues(List<float> coefficients)
        {
            if (coefficients == null || coefficients.Count < 4)
            {
                return;
            }
    
            A = coefficients[0];
            B = coefficients[1];
            C = coefficients[2];
            D = coefficients[3];
        }
        
        private void UpdateNameBasedOnType() => (A,B,C,D) = GetValueForId(Id);

        private (float A, float B, float C, float D) GetValueForId(int id)
        {
            switch (id)
            {
                case 1:
                    return (8.0472f, 0f, 0.1653f, -7.645f);
                case 2:
                    return (32.00722628f, 0f, 0.17674902f, -32.01614649f);
                case 3:
                    return (1f, 0f, 0.0498f, 0f);               
                case 4:
                    return (1f, 6.4583f, 0.047f, 0f);                
                case 5:
                    return (1f, -0.0196f, 0.0489f, 0f);                
                case 6:
                    return (1f, -0.028f, 0.0954f, 0f);               
                case 7:
                    return (1f, 0.0255f, 0.1397f, 0f);                
                case 8:
                    return (1f, 0.8053f, 0.2994f, 0f);                
                case 9:
                    return (1f, 0.223f, 0.1823f, 0f);               
                case 10:
                    return (1f, 0.3976f, 0.0097f, 0f);                
                case 11:
                    return (1f, 0.3882f, 0.0212f, 0f);
                case 12:
                    return (1f, -0.3645f, 0.1823f, 0f);
                default:
                    return (0, 0, 0, 0);
            }
        }
    }
}