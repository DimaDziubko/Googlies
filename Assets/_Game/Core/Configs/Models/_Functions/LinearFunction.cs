using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Configs.Models._Functions
{
    [Serializable]
    public class LinearFunction
    {
        public int Id;
        public float A;
        public float B;

        public LinearFunction(List<float> coefficients)
        {
            SetValues(coefficients);
        }
        
        public float GetIntValue(int level)
        {
            float result = A * level + B;
            return Mathf.Round(result);
        }

        public float GetValue(int level)
        {
            float result = A * level + B;
            return result;
        }
        
        public int GetInt(int level)
        {
            float result = A * level + B;
            return (int)Mathf.Round(result);
        }

        public short GetShort(int level)
        {
            float result = A * level + B;
            return (short)Mathf.Round(result);
        }

        public void SetValues(List<float> coefficients)
        {
            if (coefficients == null || coefficients.Count < 2)
            {
                return;
            }
    
            A = coefficients[0];
            B = coefficients[1];
        }
    }
}