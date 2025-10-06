using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Gameplay._BattleField.Scripts;

namespace _Game.Gameplay.Scenario
{
    public class EnemyWaveScheduler 
    {
        private List<EnemySpawnSequenceRunner> _spawnSequences;
        
        public State Begin(IUnitSpawnProxy unitSpawner) => new State(this, unitSpawner);

        public void Init(EnemyWave waveModel)
        {
            if (_spawnSequences == null) _spawnSequences = new List<EnemySpawnSequenceRunner>();

            for (int i = 0; i < waveModel.SpawnSequences.Count; i++)
            {
                if (i < _spawnSequences.Count)
                {
                    _spawnSequences[i].Init(waveModel.SpawnSequences[i]);
                }
                else
                {
                    EnemySpawnSequenceRunner runner = new EnemySpawnSequenceRunner();
                    runner.Init(waveModel.SpawnSequences[i]);
                    _spawnSequences.Add(runner);
                }
            }

            if (_spawnSequences.Count > waveModel.SpawnSequences.Count)
            {
                _spawnSequences.RemoveRange(waveModel.SpawnSequences.Count, 
                    _spawnSequences.Count - waveModel.SpawnSequences.Count);
            }
        }

        [Serializable]
        public struct State
        {
            private IUnitSpawnProxy _unitSpawner;
            
            private EnemyWaveScheduler _waveScheduler;
            private int _index;
            private EnemySpawnSequenceRunner.State _sequence;

            public State(EnemyWaveScheduler waveScheduler, IUnitSpawnProxy unitSpawner)
            {
                _waveScheduler = waveScheduler;
                _index = 0;
                _sequence = waveScheduler._spawnSequences[0].Begin(unitSpawner);
                _unitSpawner = unitSpawner;
            }

            public float Progress(float deltaTime)
            {
                deltaTime = _sequence.Progress(deltaTime);
                
                while (deltaTime >= 0f)
                {
                    if (++_index >= _waveScheduler._spawnSequences.Count)
                    {
                        return deltaTime;
                    }
                    _sequence = _waveScheduler._spawnSequences[_index].Begin(_unitSpawner);
                    deltaTime = _sequence.Progress(deltaTime);
                }
                
                return -1f;
            }
        }
    }
}