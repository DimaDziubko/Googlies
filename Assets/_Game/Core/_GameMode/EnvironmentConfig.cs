using _Game.Core.Configs.Build;
using System;

namespace _Game.Core._GameMode
{
    [System.Serializable]
    public class EnvironmentConfig
    {
        public EnvironmentType ActiveEnvironment { get; set; } = EnvironmentType.Development;

        public bool IsValid()
        {
            return Enum.IsDefined(typeof(EnvironmentType), ActiveEnvironment);
        }

        public EnvironmentType GetActiveEnvironment()
        {
            return ActiveEnvironment;
        }
    }
}