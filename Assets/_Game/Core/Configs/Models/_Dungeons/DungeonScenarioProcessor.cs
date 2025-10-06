using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Configs.Models._Dungeons
{
    public sealed class DungeonScenarioProcessor
    {
        private readonly ScenarioState _scenario;
        private readonly IUnitSpawnProxy _unitSpawnProxy;

        private int _currentWaveIndex;
        private int _currentSequenceIndex;
        private float _spawnCooldown;
        private float _delay;
        private int _spawnCount;

        public bool IsRunning { get; set; }
        //public event Action<GameBehavior> EnemySpawned;

        public DungeonScenarioProcessor(
            ScenarioState scenario,
            IUnitSpawnProxy unitSpawnProxy)
        {
            _scenario = scenario;
            _unitSpawnProxy = unitSpawnProxy;
        }

        public (int currentWave, int wavesCount) GetWaves()
        {
            return (_currentWaveIndex + 1, _scenario.Waves.Count + 1);
        }

        public bool Process(float deltaTime)
        {
            if (_currentWaveIndex >= _scenario.Waves.Count)
                return false;

            var wave = _scenario.Waves[_currentWaveIndex];
            var isWaveFinished = ProcessWave(wave, deltaTime);
            if (isWaveFinished)
                _currentWaveIndex++;

            return _currentWaveIndex < _scenario.Waves.Count;
        }

        private bool ProcessWave(Wave wave, float deltaTime)
        {
            var sequence = wave.Sequences[_currentSequenceIndex];
            var isSequenceFinished = ProcessSequence(sequence, deltaTime);
            if (isSequenceFinished)
            {
                _delay = 0;
                _currentSequenceIndex++;
                _spawnCount = 0;
            }

            return _currentSequenceIndex >= wave.Sequences.Count;
        }

        public bool ProcessSequence(SpawnSequence sequence, float deltaTime)
        {
            if (_delay < sequence.Delay)
            {
                _delay += deltaTime;
                return false;
            }

            _spawnCooldown += deltaTime;
            if (_spawnCooldown >= sequence.Cooldown)
            {
                _spawnCooldown = 0;
                _spawnCount++;
                SpawnEnemy(_unitSpawnProxy, sequence.UnitType);
            }

            return _spawnCount >= sequence.Count;
        }

        private void SpawnEnemy(IUnitSpawnProxy unitSpawnProxy, UnitType type)
        {
            unitSpawnProxy.SpawnEnemyUnit(type, Skin.Zombie).Forget();
        }
    }
}