using System;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class UnitSpawner : IUnitSpawner, IUnitEventsObserver
    {
        public event Action<UnitBase> UnitSpawned;
        public event Action<UnitBase> UnitDead;
        
        public GameBehaviourCollection CommonUnitCollection => _commonUnits;
        public GameBehaviourCollection ExtraUnitCollection => _extraUnits;

        private readonly IUnitFactory _unitFactory;
        private readonly IVFXProxy _vfxProxy;
        private readonly IShootProxy _shootProxy;
        private readonly IBattleSpeedManager _speedManager;
        private readonly IUnitEventHandler _unitEventHandler;

        [ShowInInspector, ReadOnly]
        private readonly GameBehaviourCollection _commonUnits = new();
        [ShowInInspector, ReadOnly]
        private readonly GameBehaviourCollection _extraUnits = new();
        
        private readonly Faction _faction;

        public UnitSpawner(
            IUnitFactory unitFactory,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            IBattleSpeedManager speedManager,
            IUnitEventHandler unitEventHandler,
            Faction faction)
        {
            _faction = faction;
            _unitFactory = unitFactory;
            _vfxProxy = vfxProxy;
            _shootProxy = shootProxy;
            _speedManager = speedManager;
            _unitEventHandler = unitEventHandler;
        }
        
        public void Cleanup()
        {
            _commonUnits.Clear();
            _extraUnits.Clear();
        }

        public void GameUpdate(float deltaTime)
        {
            _commonUnits.GameUpdate(deltaTime);
            _extraUnits.GameUpdate(deltaTime);
        }

        public void LateGameUpdate(float deltaTime)
        {
            _extraUnits.LateGameUpdate(deltaTime);
            _commonUnits.LateGameUpdate(deltaTime);
        }

        public void SetSpeedFactor(float speedFactor)
        {
            _commonUnits.SetBattleSpeedFactor(speedFactor);
            _extraUnits.SetBattleSpeedFactor(speedFactor);
        }

        public void SetPaused(bool isPaused)
        {
            _commonUnits.SetPaused(isPaused);
            _extraUnits.SetPaused(isPaused);
        }

        async UniTask<UnitBase> IUnitSpawner.SpawnUnit(UnitType type, Skin skin)
        {
            UnitBase unit = await _unitFactory.GetAsync(type, skin, _faction);

            unit.Initialize(
                _shootProxy,
                _vfxProxy,
                _speedManager.CurrentSpeedFactor,
                this);

            if(type < UnitType.SimpleUnits)
                _commonUnits.Add(unit);
            else
                _extraUnits.Add(unit);

            UnitSpawned?.Invoke(unit);

            return unit;
        }

        public void KillUnits()
        {
            _commonUnits.Clear();
            _extraUnits.Clear();
        }

        public void ResetUnits()
        {
            _commonUnits.Reset();
            _extraUnits.Reset();
        }

        void IUnitEventsObserver.NotifyDeath(UnitBase unit)
        {
            _unitEventHandler.OnUnitDead(unit);
            UnitDead?.Invoke(unit);
        }

        void IUnitEventsObserver.NotifyHit(UnitBase unit, float damage) =>
            _unitEventHandler.OnUnitHit(unit, damage);

        void IUnitEventsObserver.OnPushOut(UnitBase unit) { }
    }
}