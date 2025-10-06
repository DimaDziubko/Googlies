using System;
using System.Linq;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._Skills.Scripts;
using _Game.Utils.Timers;

namespace _Game.Gameplay._Skills
{
    public class KaboomStrategy : ISkillStrategy
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IVfxFactory _vfxFactory;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;
        private readonly ITargetRegistry _targetRegistry;

        private SynchronizedCountdownTimer _countdownTimer;

        public KaboomStrategy(
            SkillModel skillModel,
            IBattleField battleField,
            IVfxFactory vfxFactory,
            IAudioService audioService,
            IWorldCameraService cameraService,
            ITargetRegistry targetsRegistry)
        {
            _targetRegistry = targetsRegistry;
            _cameraService = cameraService;
            _skillModel = skillModel;
            _battleField = battleField;
            _vfxFactory = vfxFactory;
            _audioService = audioService;
            
            Initialize();
        }

        private void Initialize()
        {
            _countdownTimer = new SynchronizedCountdownTimer(_skillModel.GetSkillLifetime());
            _countdownTimer.TimerStop += OnStopTimer;
        }

        bool ISkillStrategy.Execute()
        {
            if(_battleField.PlayerUnitSpawner.CommonUnitCollection.IsEmpty) return false;
            
            if (_battleField.PlayerUnitSpawner.CommonUnitCollection
                .GetAllTransforms()
                .All(x => !_cameraService.IsVisibleOnCameraX(x.position)))
            {
                return false;
            }
            
            var shockwaveConfig = _skillModel.GetSpecificConfig<KaboomSkillConfig>();
            if(shockwaveConfig == null) return false;
            
            Activated?.Invoke();
            
            foreach (var behavior in _battleField.PlayerUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                
                if(unit != null && _cameraService.IsVisibleOnCameraX(unit.Position))
                {
                    KaboomView kaboomView = _vfxFactory.Get<KaboomView>(unit.SkillEffectParent);
                    KaboomDebuff kaboomDebuff = new KaboomDebuff(_targetRegistry, kaboomView, shockwaveConfig, _skillModel.Skill.Level);
                    unit.DebuffMediator.AddOrReplace(kaboomDebuff);
                }
            }
            
            PlaySfx();
            _countdownTimer.Start();
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

        private void OnStopTimer()
        {
            foreach (var behavior in _battleField.EnemyUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                if(unit != null)
                {
                    unit.DebuffMediator.Remove(DebuffType.Shockwave);
                }
            }
            
            Dispose();
        }

        private void Dispose()
        {
            _countdownTimer.TimerStop -= OnStopTimer;
            _countdownTimer.Stop();
            _countdownTimer.Dispose();
        }
    }
}