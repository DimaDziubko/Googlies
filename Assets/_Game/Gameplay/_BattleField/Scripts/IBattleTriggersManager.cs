using System;
using System.Collections.Generic;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBattleTriggersManager
    {
        void BaseDestructionStarted(Faction faction, Base @base);
        void BaseDestructionCompleted(Faction faction, Base @base);
        void AllEnemiesDefeated();
        void Register(IBattleEventHandler battleEventHandler);
        void UnRegister(IBattleEventHandler battleEventHandler);
    }

    public class BattleTriggersManager : IBattleTriggersManager, IDisposable
    {
        private readonly List<IBaseDestructionStartHandler> _baseDestructionStartHandlers = new();
        private readonly List<IBaseDestructionCompleteHandler> _baseDestructionCompleteHandlers = new();
        private readonly List<IAllEnemiesDefeatedHandler> _allEnemiesDefeatedHandlers = new();

        public void Register(IBattleEventHandler battleEventHandler)
        {
            if (battleEventHandler is IBaseDestructionStartHandler battleStartHandler)
            {
                _baseDestructionStartHandlers.Add(battleStartHandler);
            }

            if (battleEventHandler is IBaseDestructionCompleteHandler battleCompleteHandler)
            {
                _baseDestructionCompleteHandlers.Add(battleCompleteHandler);
            }

            if (battleEventHandler is IAllEnemiesDefeatedHandler allEnemiesDefeatedHandler)
            {
                _allEnemiesDefeatedHandlers.Add(allEnemiesDefeatedHandler);
            }
        }

        public void UnRegister(IBattleEventHandler battleEventHandler)
        {
            if (battleEventHandler is IBaseDestructionStartHandler battleStartHandler)
            {
                _baseDestructionStartHandlers.Remove(battleStartHandler);
            }

            if (battleEventHandler is IBaseDestructionCompleteHandler battleCompleteHandler)
            {
                _baseDestructionCompleteHandlers.Remove(battleCompleteHandler);
            }

            if (battleEventHandler is IAllEnemiesDefeatedHandler allEnemiesDefeatedHandler)
            {
                _allEnemiesDefeatedHandlers.Remove(allEnemiesDefeatedHandler);
            }
        }

        void IBattleTriggersManager.BaseDestructionStarted(Faction faction, Base @base)
        {
            foreach (var handler in _baseDestructionStartHandlers)
            {
                handler.OnBaseDestructionStarted(faction, @base);
            }
        }

        void IBattleTriggersManager.BaseDestructionCompleted(Faction faction, Base @base)
        {
            foreach (var handler in _baseDestructionCompleteHandlers)
            {
                handler.OnBaseDestructionCompleted(faction, @base);
            }
        }

        void IBattleTriggersManager.AllEnemiesDefeated()
        {
            foreach (var handler in _allEnemiesDefeatedHandlers)
            {
                handler.OnAllUnitsDefeated();
            }
        }

        void IDisposable.Dispose()
        {
            _baseDestructionCompleteHandlers.Clear();
            _baseDestructionStartHandlers.Clear();
            _allEnemiesDefeatedHandlers.Clear();
        }
    }
}