using System;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._Skills.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._Skills
{
    public class GhostsStrategy : ISkillStrategy
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _model;
        private readonly IUnitSpawnProxy _spawnProxy;
        private readonly IAudioService _audioService;


        public GhostsStrategy(
            SkillModel model, 
            IUnitSpawnProxy spawnProxy, 
            IAudioService audioService)
        {
            _model = model;
            _spawnProxy = spawnProxy;
            _audioService = audioService;
        }

        bool ISkillStrategy.Execute()
        {
            var hugsConfig = _model.GetSpecificConfig<GhostsSkillConfig>();
            if(hugsConfig == null) return false;
            Activated?.Invoke();
            
            SpawnHugsUnits(hugsConfig);
            PlaySfx();
            Completed?.Invoke();
            return true;
        }

        void ISkillStrategy.Interrupt() { }

        private void SpawnHugsUnits(GhostsSkillConfig config)
        {
            _spawnProxy.SpawnPlayerUnit(UnitType.Ghosts, Skin.GreenGhost).Forget();
            _spawnProxy.SpawnPlayerUnit(UnitType.Ghosts, Skin.BlueGhost).Forget();
            _spawnProxy.SpawnPlayerUnit(UnitType.Ghosts, Skin.MagentaGhost).Forget();
        }

        private void PlaySfx()
        {
            if (_audioService != null && _model.Sfx != null)
            {
                _audioService.PlayOneShot(_model.Sfx);
            }
        }
    }
}