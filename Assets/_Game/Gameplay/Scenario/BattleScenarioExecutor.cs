using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Gameplay._BattleField.Scripts;

namespace _Game.Gameplay.Scenario
{
    public class BattleScenarioExecutor 
    {
        private List<EnemyWaveScheduler> _waves;
        
        public State Begin(IUnitSpawnProxy unitSpawner) => new(this, unitSpawner);
        
        public void UpdateScenario(BattleScenario scenarioData)
        {
            if (_waves == null) _waves = new List<EnemyWaveScheduler>();
            
            for (int i = 0; i < scenarioData.Waves.Count; i++)
            {
                if (i < _waves.Count)
                {
                    _waves[i].Init(scenarioData.Waves[i]);
                }
                else
                {
                    EnemyWaveScheduler waveScheduler = new EnemyWaveScheduler();
                    waveScheduler.Init(scenarioData.Waves[i]);
                    _waves.Add(waveScheduler);
                }
            }
            
            if (_waves.Count > scenarioData.Waves.Count)
            {
                _waves.RemoveRange(scenarioData.Waves.Count, _waves.Count - scenarioData.Waves.Count);
            }
        }

        [Serializable]
        public struct State
        {
            private IUnitSpawnProxy _unitSpawner;
            
            private BattleScenarioExecutor _scenarioExecutor;
            private int _index;
            private EnemyWaveScheduler.State _wave;

            public (int currentWave, int wavesCount) GetWaves()
            {
                return (_index + 1, _scenarioExecutor._waves.Count);
            }

            public State(BattleScenarioExecutor scenarioExecutor, IUnitSpawnProxy unitSpawner)
            {
                _scenarioExecutor = scenarioExecutor;
                _index = 0;
                _wave = _scenarioExecutor._waves[0].Begin(unitSpawner);
                _unitSpawner = unitSpawner;
            }

            public bool Progress(float tickDeltaTime)
            {
                float deltaTime = _wave.Progress(tickDeltaTime);
                while (deltaTime >= 0f)
                {
                    if (++_index >= _scenarioExecutor._waves.Count)
                    {
                        return false;
                    }

                    _wave = _scenarioExecutor._waves[_index].Begin(_unitSpawner);
                    deltaTime = _wave.Progress(tickDeltaTime);
                }

                return true;
            }
        }
    }
}