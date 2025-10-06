using System;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._Skills.Scripts;

namespace _Game.Gameplay._Skills
{
    public class HornStrategy : ISkillStrategy
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IVfxFactory _vfxFactory;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;

        public HornStrategy(
            SkillModel skillModel, 
            IBattleField battlefield,
            IVfxFactory vfxFactory, 
            IAudioService audioService,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _skillModel = skillModel;
            _battleField = battlefield;
            _vfxFactory = vfxFactory;
            _audioService = audioService;
            _logger = logger;
        }

        bool ISkillStrategy.Execute()
        {
            if(_battleField.PlayerUnitSpawner.CommonUnitCollection.IsEmpty) return false;
            if(_battleField.PlayerUnitSpawner.CommonUnitCollection.GetAllTransforms().All(x => !_cameraService.IsVisibleOnCameraX(x.position))) return false;

            
            var shineConfig = _skillModel.GetSpecificConfig<HornSkillConfig>();
            if(shineConfig == null) return false;
            
            Activated?.Invoke();
            
            foreach (var behavior in _battleField.PlayerUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                
                if(unit != null && _cameraService.IsVisibleOnCameraX(unit.Position))
                {
                    HornView hornView = _vfxFactory.Get<HornView>(unit.SkillEffectParent);
                    ShineBuff shineBuff = new ShineBuff(hornView, shineConfig, _logger, _skillModel.Skill.Level);
                    unit.DebuffMediator.AddOrReplace(shineBuff);
                }
            }
            
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