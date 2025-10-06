using System;
using _Game.Gameplay._Units.Scripts.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class UnitBasePusher : MonoBehaviour
    {
        public event Action OnUnitPushedOut;
        
        private IMovable _movable;

        [ShowInInspector, ReadOnly] private float _checkInterval = 0.1f;
        [ShowInInspector, ReadOnly] private float _stopThreshold = 0.05f;
        [ShowInInspector, ReadOnly] private float _offset = 1f;

        [ShowInInspector] private bool _isInsideBase;
        
        public bool IsInsideBase => _isInsideBase;
        
        private float _baseBoundaryX;
        private bool _isLeftSide;
        private float _nextCheckTime;
        
        public void Construct(IMovable movable) => 
            _movable = movable;

        public void Initialize(
            float baseBoundaryX,
            bool isLeftSide,
            bool isInsideBase)
        {
            _baseBoundaryX = baseBoundaryX;
            _isLeftSide = isLeftSide;
            _isInsideBase = isInsideBase;
        }
        
        public void SetInsideBase(bool value)
        {
            _isInsideBase = value;
        }
        
        public void GameUpdate(float deltaTime)
        {
            if (!_isInsideBase) return;
            if (Time.time < _nextCheckTime) return;

            _nextCheckTime = Time.time + _checkInterval;

            if (!_movable.IsMoving)
            {
                _movable.Move(_isLeftSide
                    ? new Vector3(_baseBoundaryX + _offset, _movable.Position.y, _movable.Position.z)
                    : new Vector3(_baseBoundaryX - _offset, _movable.Position.y, _movable.Position.z));
            }
            
            if ((_isLeftSide && _movable.Position.x >= _baseBoundaryX) ||
                (!_isLeftSide && _movable.Position.x <= _baseBoundaryX))
            {
                _isInsideBase = false;
                _movable.Move(_movable.Position);
                OnUnitPushedOut?.Invoke();
            }
        }
    }
}