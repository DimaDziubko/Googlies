using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._Logger;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class BaseSpawner : IBaseSpawner
    {
        private readonly IBaseFactory _baseFactory;
        private readonly ILootCoinSpawner _lootCoinSpawner;
        private readonly IBaseDataProvider _baseDataProvider;
        private readonly IBaseEventHandler _baseEventHandler;
        private readonly IMyLogger _logger;
        private readonly Faction _faction;

        private Base _base;

        private Quaternion _rotation;
        private Vector3 _position;

        private bool _isUpdating;

        public BaseSpawner(
            IBaseFactory baseFactory,
            IBaseDataProvider baseDataProvider,
            IBaseEventHandler baseEventHandler,
            IMyLogger logger,
            Faction faction)
        {
            _faction = faction;
            _baseFactory = baseFactory;
            _baseEventHandler = baseEventHandler;
            _baseDataProvider = baseDataProvider;
            _logger = logger;
        }

        public void OnStartBattle()
        {
            var baseData = _baseDataProvider.GetBaseData(_faction);
            _base.UpdateHealth(baseData.Health);
            _base.UpdateCoins(baseData.CoinsAmount);
            _base.ShowHealth();
        }

        public void Init(
            Vector3 position,
            Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
        }
        
        public async UniTask UpdateState()
        {
            if (_isUpdating)
            {
                _logger.Log($"{_faction} UpdateState() call was skipped — already updating", DebugStatus.Warning);
                return;
            }

            _isUpdating = true;
            try
            {
                _logger.Log($"UPDATE BASE DATA {_faction}");
        
                RemoveBase();
        
                _base = await _baseFactory.GetAsync(_faction);
        
                _base.PrepareIntro(_baseEventHandler, _position, _rotation);
                _base.Reset();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public void SetPaused(bool isPaused)
        {
            if (_base != null) 
                _base.SetPaused(isPaused);
        }

        private void RemoveBase()
        {
            if (_base != null)
            {
                _base.Recycle();
                _base = null;
            }
        }
        
        public void Cleanup()
        {
            RemoveBase();
            _baseFactory.Cleanup();
        }
    }
}