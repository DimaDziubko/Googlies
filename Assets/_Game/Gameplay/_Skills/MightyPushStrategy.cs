using System;
using System.Linq;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Units.Scripts.Movement;
using _Game.UI._Skills.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Skills
{
    public class MightyPushStrategy : ISkillStrategy
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IAudioService _audioService;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;

        public MightyPushStrategy(
            SkillModel model,
            IBattleField battlefield, 
            IAudioService audioService,
            IUnitDataProvider unitDataProvider,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _skillModel = model;
            _battleField = battlefield;
            _audioService = audioService;
            _unitDataProvider = unitDataProvider;
            _logger = logger;
        }

        bool ISkillStrategy.Execute()
        {
            if(_battleField.EnemyUnitSpawner.CommonUnitCollection.IsEmpty) return false;
            
            if (_battleField.EnemyUnitSpawner.CommonUnitCollection
                .GetAllTransforms()
                .All(x => !_cameraService.IsVisibleOnCameraX(x.position)))
            {
                return false;
            }
            
            var windyConfig = _skillModel.GetSpecificConfig<MightyPushSkillConfig>();
            if(windyConfig == null) return false;
            
            Activated?.Invoke();
            
            IUnitData startingUnitData = _unitDataProvider.GetDecoratedUnitData(Faction.Player, UnitType.Light, Skin.Ally);
            float damageToApply = _skillModel.GetSkillValue() * startingUnitData.WeaponData.Damage;  
   
            foreach (var behavior in _battleField.EnemyUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                
                if(unit != null && _cameraService.IsVisibleOnCameraX(unit.Position))
                {
                    unit.Health.GetDamage(damageToApply);
                    unit.PushComponent.Push(Vector2.right, windyConfig.ImpulseStrength, PushType.Forced);
                }
            }

            _battleField.VFXProxy.SpawnPush(new Vector3(-1.5f, 0, 0));
            PlaySfx();
            Completed?.Invoke();
            return true;
        }
        
        void ISkillStrategy.Interrupt() { }

        private void PlaySfx()
        {
            var clip = _skillModel.Sfx;
            if (clip != null)
            {
                _audioService.PlayOneShot(_skillModel.Sfx);
            }
        }
    }
}