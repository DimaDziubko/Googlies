using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Common
{
    [Serializable]
    public class GameBehaviourCollection 
    {
        [ShowInInspector, ReadOnly]
        private List<GameBehaviour> _behaviors = new();
        private List<Transform> _transforms = new();
        
        public IEnumerable<GameBehaviour> Behaviours => _behaviors;
        
        public bool IsEmpty => _behaviors.Count == 0;

        public int Count
        {
            get => _behaviors.Count;
        }
        
        public void Add(GameBehaviour behaviour)
        {
            _behaviors.Add(behaviour);
            _transforms.Add(behaviour.transform);
        }

        public void GameUpdate(float deltaTime)
        {
            for (var i = 0; i < _behaviors.Count; i++)
            {
                if (_behaviors[i].GameUpdate(deltaTime) == false)
                {
                    var lastIndex = _behaviors.Count - 1;
                    _behaviors[i] = _behaviors[lastIndex];
                    _transforms[i] = _transforms[lastIndex];
                    _behaviors.RemoveAt(lastIndex);
                    _transforms.RemoveAt(lastIndex);
                    i--;
                }
            }
        }
        
        public void LateGameUpdate(float deltaTime)
        {
            for (var i = 0; i < _behaviors.Count; i++)
            {
                if (_behaviors[i].LateGameUpdate(deltaTime) == false)
                {
                    var lastIndex = _behaviors.Count - 1;
                    _behaviors[i] = _behaviors[lastIndex];
                    _transforms[i] = _transforms[lastIndex];
                    _behaviors.RemoveAt(lastIndex);
                    _transforms.RemoveAt(lastIndex);
                    i--;
                }
            }
        }
        
        public void SetPaused(in bool isPaused)
        {
            foreach (var t in _behaviors)
            {
                t.SetPaused(isPaused);
            }
        }

        public void Reset()
        {
            foreach (var t in _behaviors)
            {
                t.Reset();
            }
        }

        public void SetBattleSpeedFactor(float speedFactor)
        {
            for (var i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].SetSpeedFactor(speedFactor);
            }
        }

        public void Clear()
        {
            foreach (var t in _behaviors)
            {
                t.Recycle();
            }

            _behaviors.Clear();
            _transforms.Clear();
        }

        public Transform[] GetAllTransforms() => _transforms.ToArray();
    }
}
