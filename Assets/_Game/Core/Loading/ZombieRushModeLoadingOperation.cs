using System;
using System.Collections.Generic;
using _Game._BattleModes.Scripts;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Game.Core.Loading
{
    public sealed class ZombieRushModeLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IDungeonModel _model;
        private readonly IMyLogger _logger;

        public string Description => "Rats rush loading...";
        
        public ZombieRushModeLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IDungeonModel model,
            IMyLogger logger)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _model = model;
            _logger = logger;
        }
        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            onProgress?.Invoke(0.5f);
            var loadOp = _sceneLoader.LoadSceneAsync(Constants.Scenes.ZOMBIE_RUSH_MODE,
                LoadSceneMode.Additive);
            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            
            onProgress?.Invoke(0.7f);
            
            var scene = SceneManager.GetSceneByName(Constants.Scenes.ZOMBIE_RUSH_MODE);
            var sceneContext = scene.GetRoot<SceneContext>();
            var zombieRushMode = sceneContext.Container.Resolve<ZombieRushMode>();

            onProgress?.Invoke(0.8f);
            
            var generator = new DungeonScenarioGenerator();
            
            var scenarioTree = CreateTree();

            var dungeonScenarioState = generator.Generate(scenarioTree);

            await zombieRushMode.Initialize(_model, dungeonScenarioState);
            
            onProgress?.Invoke(0.9f);
            
            _cameraService.EnableMainCamera();
            zombieRushMode.BeginNewGame();
                
            onProgress?.Invoke(1.0f);
            
#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }

   private ScenarioNode CreateTree()
        {
            var scenarioTree = new ScenarioNode("Scenario");
            
            ScenarioNode wave = new ScenarioNode("Wave");
            
            
            short lightWarriorsCount = _model.GetWarriorsCount(UnitType.Light);
            
            _logger.Log($"LIGHT WARRIORS COUNT {lightWarriorsCount}", DebugStatus.Info);
            
            if (lightWarriorsCount > 0)
            {
                wave.AddChild(new ScenarioNode("Sequence", null)
                {
                    Children = new List<ScenarioNode>()
                    {
                        new("UnitType", UnitType.Light),
                        new("Count", lightWarriorsCount),
                        new("Cooldown", _model.SequenceCooldown),
                        new("Delay", _model.Delay)
                    }
                });
            }
            
            
            short mediumWarriorsCount = _model.GetWarriorsCount(UnitType.Medium);
            
            _logger.Log($"MEDIUM WARRIORS COUNT {mediumWarriorsCount}", DebugStatus.Info);
            
            if (mediumWarriorsCount > 0)
            {
                wave.AddChild(new ScenarioNode("Sequence", null)
                {
                    Children = new List<ScenarioNode>()
                    {
                        new("UnitType", UnitType.Medium),
                        new("Count", mediumWarriorsCount),
                        new("Cooldown", _model.SequenceCooldown),
                        new("Delay", 0)
                    }
                });

            }
            
            short heavyWarriorsCount = _model.GetWarriorsCount(UnitType.Heavy);
            
            _logger.Log($"HEAVY WARRIORS COUNT {heavyWarriorsCount}", DebugStatus.Info);
            
            if (heavyWarriorsCount > 0)
            {
                wave.AddChild(new ScenarioNode("Sequence", null)
                {
                    Children = new List<ScenarioNode>()
                    {
                        new("UnitType", UnitType.Heavy),
                        new("Count", heavyWarriorsCount),
                        new("Cooldown", _model.SequenceCooldown),
                        new("Delay", 0)
                    }
                });
            }
            
            scenarioTree.AddChild(wave);
            
            return scenarioTree;
        }
    }
}