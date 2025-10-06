using UnityEngine;

namespace _Game.Core.Services.Random
{
    public interface IRandomService 
    {
        int Next(int intMin,int intMax);
        float Next(float min, float max);
        float GetValue();
        Vector3 OnUnitSphere();
    }
}