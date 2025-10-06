using System;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay.Scenario
{
    public class EnemySpawnSequenceRunner
    {
        private UnitType _type;
        private int _amount;
        private float _cooldown;
        private float _startDelay;

        public void Init(EnemySpawnSequence sequence)
        {
            _type = sequence.Type;
            _amount = sequence.Amount;
            _cooldown = sequence.Cooldown;
            _startDelay = sequence.StartDelay;
        }

        public State Begin(IUnitSpawnProxy _unitSpawner) => new(this, _unitSpawner, _startDelay);

        [Serializable]
        public struct State
        {
            private IUnitSpawnProxy _unitSpawner;
            
            private EnemySpawnSequenceRunner _sequenceRunner;
            private int _count;
            private float _cooldown;
            private float _startDelay;

            public State(EnemySpawnSequenceRunner sequenceRunner, IUnitSpawnProxy unitSpawner, float startDelay)
            {
                _sequenceRunner = sequenceRunner;
                _count = 0;
                _cooldown = sequenceRunner._cooldown;
                _unitSpawner = unitSpawner;
                _startDelay = startDelay;
            }

            public float Progress(float deltaTime)
            {
                _cooldown += deltaTime;
                
                if (_startDelay > 0)
                {
                    if (_cooldown < _sequenceRunner._cooldown)
                    {
                        return -1f;
                    }
                    
                    _startDelay -= _cooldown;
                    _cooldown = 0;
                }

                
                while (_cooldown >= _sequenceRunner._cooldown)
                {
                    _cooldown -= _sequenceRunner._cooldown;
                    if (_count >= _sequenceRunner._amount)
                    {
                        return _cooldown;
                    }
                    _count += 1;
                    
                    _unitSpawner.SpawnEnemyUnit(_sequenceRunner._type, Skin.Hostile).Forget();
                }
                return -1f;
            }
        }
    }
}