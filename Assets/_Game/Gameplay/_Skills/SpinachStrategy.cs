using System;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._Skills.Scripts;

namespace _Game.Gameplay._Skills
{
    public class SpinachStrategy : 
        ISkillStrategy, 
        IStopGameListener,
        IUnitEventsObserver
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _model;
        private readonly IBattleField _battleField;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private SpinachSkillConfig _spinachConfig;
        
        private UnitBase _targetUnit;


        public SpinachStrategy(
            SkillModel model, 
            IBattleField battlefield,
            IAudioService audioService, 
            IMyLogger logger)
        {
            _model = model;
            _battleField = battlefield;
            _audioService = audioService;
            _logger = logger;
        }

        bool ISkillStrategy.Execute()
        {
            _spinachConfig = _model.GetSpecificConfig<SpinachSkillConfig>();
            if(_spinachConfig == null) return false;
            
            Activated?.Invoke();
            
            _battleField.PlayerUnitSpawner.UnitSpawned += OnUnitSpawned;
            
            PlaySfx();
            return true;
        }

        void ISkillStrategy.Interrupt()
        {
            _battleField.PlayerUnitSpawner.UnitSpawned -= OnUnitSpawned;
            _targetUnit = null;
        }

        void IStopGameListener.OnStopBattle()
        {
            _battleField.PlayerUnitSpawner.UnitSpawned -= OnUnitSpawned;
        }

        private void OnUnitSpawned(UnitBase unit)
        {
            if(unit.Type >= UnitType.SimpleUnits || _targetUnit != null) return;
            
            _targetUnit = unit;
            unit.Dispatcher.RegisterEventObserver(this);
        }
        

        private void PlaySfx()
        {
            if (_audioService != null && _model.Sfx != null)
            {
                _audioService.PlayOneShot(_model.Sfx);
            }
        }

        void IUnitEventsObserver.NotifyDeath(UnitBase unit)
        {
            if (_targetUnit != null && _targetUnit == unit)
            {
                Completed?.Invoke();
            }
        }

        void IUnitEventsObserver.NotifyHit(UnitBase unit, float damage) { }
        
        void IUnitEventsObserver.OnPushOut(UnitBase unit)
        {
            if (_targetUnit != null && _targetUnit == unit)
            {
                _battleField.PlayerUnitSpawner.UnitSpawned -= OnUnitSpawned;
                StrengthBuff buff = new StrengthBuff(_spinachConfig, _logger, _model.Skill.Level);
                unit.DebuffMediator.AddOrReplace(buff);
                Completed?.Invoke();
            }
        }
    }
}