using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Units.FSM.States
{
    public class MoveToPointWarriorState : WarriorStateBase
    {
        private const float MAX_PATH_DEVIATION = 0.5f;
        
        private const float DECISION_TIME_MIN = 5;
        private const float DECISION_TIME_MAX = 10;
        
        private const float MIN_DISTANCE_TO_DEVIATION_POINT = 0.5f;
        
        private const float NOISE_FREQUENCY = 1f;

        private readonly IRandomService _random;
        private readonly UnitBase _unit;
        
        private float _lastDecisionTime;

        private Vector3 _currentTarget;
        private float _lastPathUpdateTime;

        private float _pathUpdateFrequency;
        
        public MoveToPointWarriorState(UnitBase unit, IRandomService random, string name) : base(unit, name)
        {
            _unit = unit;
            _random = random;
        }


        public override void Enter()
        {
            CalculateRandomPathUpdateFrequency();
            _unit.CalmDown();
            _lastPathUpdateTime = -float.MaxValue;
        }

        public override void GameUpdate(float deltaTime)
        {
            if (NeedNewPath()) UpdatePath();
            _unit.AMove.Move(_currentTarget);
            
            _unit.Animator.PlayBottomLocomotion(_unit.AMove.GetCurrentSpeed());
            _unit.Animator.PlayTopLocomotion(_unit.AMove.GetCurrentSpeed());
        }

        public override void Exit() { }

        private void CalculateRandomPathUpdateFrequency() => 
            _pathUpdateFrequency = _random.Next(DECISION_TIME_MIN, DECISION_TIME_MAX);

        private bool NeedNewPath()
        {
            return Time.time - _lastPathUpdateTime >= _pathUpdateFrequency
                   || Vector3.Distance(_unit.AMove.Position, _currentTarget) <= MIN_DISTANCE_TO_DEVIATION_POINT
                   || !_unit.AMove.IsActuallyMoving;
        }

        private void UpdatePath()
        {
            _currentTarget = GetRandomizedPath(_unit.AMove.Position, _unit.Destination, NOISE_FREQUENCY, MAX_PATH_DEVIATION);
            _lastPathUpdateTime = Time.time;
            
            CalculateRandomPathUpdateFrequency();
        }

        private Vector3 GetRandomizedPath(Vector3 start, Vector3 end, float noiseFrequency, float maxDeviation)
        {
            float deviationScale = 2.0f;
            float noiseOffset = 1.0f; 
            float pathMidpoint = 0.5f; 

            float noiseValue = Mathf.PerlinNoise(Time.time * noiseFrequency, 0.0f) * deviationScale - noiseOffset; 
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward); 
            Vector3 deviation = perpendicular * noiseValue * maxDeviation; 

            Vector3 targetPoint = Vector3.Lerp(start, end, pathMidpoint) + deviation;
            return targetPoint;
        }

        public override void Cleanup()
        {
            _lastPathUpdateTime = -float.MaxValue;
        }
    }
}