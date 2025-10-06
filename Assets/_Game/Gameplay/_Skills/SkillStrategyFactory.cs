using System;
using _Game.Common;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Repositories;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay.Common;
using _Game.Gameplay.Food.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI._Skills.Scripts;

namespace _Game.Gameplay._Skills
{
    public class SkillStrategyFactory
    {
        private readonly IBattleField _battlefield;
        private readonly IVfxFactory _vfxFactory;
        private readonly IPauseManager _pauseManager;
        private readonly IAudioService _audioService;
        private readonly IUnitSpawnProxy _spawnProxy;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IConfigRepository _config;
        private readonly IFoodContainer _foodContainer;
        private readonly IWorldCameraService _cameraService;
        private readonly ITargetRegistry _targetRegistry;
        private readonly IMyLogger _logger;

        public SkillStrategyFactory(
            IBattleField battlefield,
            IVfxFactory vfxFactory,
            IPauseManager pauseManager,
            IAudioService audioService,
            IUnitSpawnProxy spawnProxy,
            IUnitDataProvider unitDataProvider,
            ICoroutineRunner coroutineRunner,
            IConfigRepository config,
            IFoodContainer foodContainer,
            IWorldCameraService cameraService,
            ITargetRegistry targetRegistry,
            IMyLogger logger)
        {
            _battlefield = battlefield;
            _vfxFactory = vfxFactory;
            _pauseManager = pauseManager;
            _audioService = audioService;
            _spawnProxy = spawnProxy;
            _unitDataProvider = unitDataProvider;
            _coroutineRunner = coroutineRunner;
            _config = config;
            _foodContainer = foodContainer;
            _cameraService = cameraService;
            _targetRegistry = targetRegistry;
            _logger = logger;
        }

        public ISkillStrategy GetStrategy(SkillModel model)
        {
            return model.Type switch
            {
                SkillType.Freeze => new FreezeStrategy(model, _battlefield, _pauseManager, _vfxFactory, _audioService, _cameraService, _logger),
                SkillType.Kaboom => new KaboomStrategy(model, _battlefield, _vfxFactory, _audioService, _cameraService, _targetRegistry),
                SkillType.Ghosts => new GhostsStrategy(model, _spawnProxy, _audioService),
                SkillType.Horn => new HornStrategy(model, _battlefield, _vfxFactory, _audioService, _cameraService, _logger),
                SkillType.Spinach => new SpinachStrategy(model, _battlefield, _audioService, _logger),
                SkillType.Meteors => new MeteorsStrategy(model, _battlefield, _audioService, _unitDataProvider, _coroutineRunner, _logger),
                SkillType.MightyPush => new MightyPushStrategy(model, _battlefield, _audioService, _unitDataProvider, _cameraService, _logger),
                SkillType.Recover => new RecoverStrategy(model, _battlefield, _audioService, _foodContainer, _config.IconConfigRepository, _cameraService, _logger),
                _ => throw new ArgumentOutOfRangeException($"Unsupported Type: {model.Type}")
            };
        }
    }
}