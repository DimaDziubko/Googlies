using _Game.Gameplay._PickUp;
using _Game.Gameplay._PickUp._PickUpFactory;
using _Game.Gameplay.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IPickUpProxy
    {
        PickUp SpawnPickUp(Vector3 position);
    }
    
    public class PickUpSpawner : IPickUpProxy
    {
        private readonly IPickUpFactory _pickUpFactory;
        
        [ShowInInspector, ReadOnly]
        private readonly GameBehaviourCollection _powerUps = new();

        public PickUpSpawner(IPickUpFactory pickUpFactory)
        {
            _pickUpFactory = pickUpFactory;
        }
        
        public void GameUpdate(float deltaTime) => 
            _powerUps.GameUpdate(deltaTime);
        
        public PickUp SpawnPickUp(Vector3 position)
        {
            var pickUp = _pickUpFactory.GetPickUp();
            pickUp.Position = position;
            _powerUps.Add(pickUp);
            return pickUp;
        }

        public void Cleanup() => _powerUps.Clear();
    }
}