using System;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Skills.Scripts;
using _Game.Utils.Extensions;

namespace _Game.Gameplay._Skills
{
    public class RecoverStrategy : ISkillStrategy
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IAudioService _audioService;
        private readonly IFoodContainer _foodContainer;
        private readonly IIconConfigRepository _iconConfig;
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;

        public RecoverStrategy(
            SkillModel skillModel, 
            IBattleField battlefield,
            IAudioService audioService,
            IFoodContainer foodContainer,
            IIconConfigRepository iconConfig,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _skillModel = skillModel;
            _battleField = battlefield;
            _audioService = audioService;
            _foodContainer = foodContainer;
            _iconConfig = iconConfig;
            _cameraService = cameraService;
            _logger = logger;
        }

        bool ISkillStrategy.Execute()
        {
            if(_battleField.PlayerUnitSpawner.CommonUnitCollection.IsEmpty) return false;
            if(_battleField.PlayerUnitSpawner.CommonUnitCollection.GetAllTransforms().All(x => !_cameraService.IsVisibleOnCameraX(x.position))) return false;
            
            var recoverSkillConfig = _skillModel.GetSpecificConfig<RecoverSkillConfig>();
            if(recoverSkillConfig == null) return false;
   
            Activated?.Invoke();
            
            foreach (var behavior in _battleField.PlayerUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                
                if(unit != null && _cameraService.IsVisibleOnCameraX(unit.Position))
                {
                    float foodToRecover = unit.UnitData.FoodPrice * _skillModel.GetSkillValue();
                    _battleField.VFXProxy.SpawnUnitVfx(unit.Position);
                    _battleField.VFXProxy.SpawnRecover(unit.Position, _iconConfig.FoodIcon(), foodToRecover.ToCompactFormat(999));
                    _foodContainer.Add((int)foodToRecover);
                    behavior.Recycle();
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