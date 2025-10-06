using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface ILootCoinSpawner
    {
        void SpawnLootCoin(Vector3 position, float amount);
    }
}