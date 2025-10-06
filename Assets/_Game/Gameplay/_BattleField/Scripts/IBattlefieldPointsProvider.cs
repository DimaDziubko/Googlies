using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBattlefieldPointsProvider
    {
        Vector3 PlayerSpawnPoint { get; }
        Vector3 EnemySpawnPoint { get; }
        Vector3 EnemyDestinationPoint { get; }
        Vector3 PlayerDestinationPoint { get; }
        Vector3 PlayerBaseSpawnPoint { get; }
        Vector3 EnemyBaseSpawnPoint { get; }
    }
}