using System;
using System.Threading.Tasks;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitSpawner
    {
        event Action<UnitBase> UnitSpawned;
        event Action<UnitBase> UnitDead;

        GameBehaviourCollection CommonUnitCollection { get; }
        GameBehaviourCollection ExtraUnitCollection { get; }
        UniTask<UnitBase> SpawnUnit(UnitType type, Skin skin);
    }
}