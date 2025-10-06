using System;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using _Game.UI._Skills.Scripts;
using ImprovedTimers;

namespace _Game.Gameplay._Skills
{
    public class FreezeStrategy : ISkillStrategy, IPauseHandler
    {
        public event Action Activated;
        public event Action Completed;

        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IPauseManager _pauseManager;
        private readonly IVfxFactory _vfxFactory;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;

        private CountdownTimer _countdownTimer;

        public FreezeStrategy(
            SkillModel skillModel,
            IBattleField battleField,
            IPauseManager pauseManager,
            IVfxFactory vfxFactory,
            IAudioService audioService,
            IWorldCameraService cameraService, 
            IMyLogger logger)
        {
            _skillModel = skillModel;
            _battleField = battleField;
            _pauseManager = pauseManager;
            _vfxFactory = vfxFactory;
            _audioService = audioService;
            _logger = logger;
            _cameraService = cameraService;

            Initialize();
        }


        private void Initialize()
        {
            _pauseManager.AddHandler(this);
            _countdownTimer = new CountdownTimer(_skillModel.Duration);
            _countdownTimer.TimerStop += OnStopTimer;
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
            
            Activated?.Invoke();

            foreach (var behavior in _battleField.EnemyUnitSpawner.CommonUnitCollection.Behaviours)
            {
                UnitBase unit = behavior as UnitBase;
                if (unit != null && _cameraService.IsVisibleOnCameraX(unit.Position))
                {
                    IceDebuffView iceDebuffView = _vfxFactory.Get<IceDebuffView>(unit.SkillEffectParent);
                    IceDebuff iceDebuff = new IceDebuff(iceDebuffView);
                    unit.DebuffMediator.AddOrReplace(iceDebuff);
                }
            }

            PlaySfx();

            _countdownTimer.Reset();
            _countdownTimer.Start();
            _logger.Log($"START FREEZE {_countdownTimer.CurrentTime}", DebugStatus.Info);

            return true;
        }

        void ISkillStrategy.Interrupt()
        {
        }

        public void SetPaused(bool isPaused)
        {
            if (isPaused)
                _countdownTimer.Stop();
            else
                _countdownTimer.Start();
        }

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

                if (unit != null)
                {
                    unit.DebuffMediator.Remove(DebuffType.Ice);
                }
            }

            Dispose();
            _logger.Log($"STOP FREEZE {_countdownTimer.CurrentTime}", DebugStatus.Info);

            Completed?.Invoke();
        }

        private void Dispose()
        {
            _pauseManager.RemoveHandler(this);
            _countdownTimer.TimerStop -= OnStopTimer;
            _countdownTimer.Stop();
            _countdownTimer.Dispose();
        }
    }
}