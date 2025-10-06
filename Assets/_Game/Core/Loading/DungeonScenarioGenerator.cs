using System.Collections.Generic;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.Loading
{
    public class ScenarioNode
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public List<ScenarioNode> Children { get; set; } = new();
    
        public ScenarioNode(string key, object value = null)
        {
            Key = key;
            Value = value;
        }

        public void AddChild(ScenarioNode child)
        {
            Children.Add(child);
        }
    }
    
    public class DungeonScenarioGenerator
    {
        public ScenarioState Generate(ScenarioNode root)
        {
            ScenarioState state = new ScenarioState()
            {
                Waves = new List<Wave>()
            };

            foreach (var waveNode in root.Children)
            {
                if (waveNode.Key == "Wave")
                {
                    var wave = GenerateWave(waveNode);
                    state.Waves.Add(wave);
                }
            }

            return state;
        }

        private Wave GenerateWave(ScenarioNode waveNode)
        {
            Wave wave = new Wave()
            {
                Sequences = new List<SpawnSequence>()
            };

            foreach (var sequenceNode in waveNode.Children)
            {
                var sequence = GenerateSequence(sequenceNode);
                wave.Sequences.Add(sequence);
            }

            return wave;
        }

        private SpawnSequence GenerateSequence(ScenarioNode sequenceNode)
        {
            var unitType = (UnitType)sequenceNode.Children.Find(c => c.Key == "UnitType").Value; 
            var count = (short)(sequenceNode.Children.Find(c => c.Key == "Count").Value ?? 0);
            var cooldown = (int)(sequenceNode.Children.Find(c => c.Key == "Cooldown").Value ?? 0);
            var delay = (int)(sequenceNode.Children.Find(c => c.Key == "Delay").Value ?? 0);

            return new SpawnSequence()
            {
                UnitType = unitType,
                Count = count,
                Cooldown = cooldown,
                Delay = delay
            };
        }
    }
}