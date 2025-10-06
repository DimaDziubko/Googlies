using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Models._ScenarioConfig
{
    public class SpawnSequence
    {
        public int Id;
        public UnitType UnitType;
        public short Count;
        public float Cooldown;
        public float Delay = 0;
    }
}