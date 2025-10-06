using _Game.Core._GameListenerComposite;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Core.Configs.Models._ScenarioConfig;
using _Game.Core.CustomKernel;
using _Game.Gameplay._Battle.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game._BattleModes.Scripts
{
    public class ZombieRushBattle : 
        IStartGameListener, 
        IStopGameListener, 
        IGameTickable,
        IGameLateTickable,
        IPauseListener
    { 
        private readonly ZombieRushBattleField _battleField;
        private readonly AmbienceController _ambienceController;
        
        private DungeonScenarioProcessor _processor;
        
        private bool _isPaused;

        public ZombieRushBattle(
            ZombieRushBattleField battleField,
            AmbienceController ambienceController)
        {
            _battleField = battleField;
            _ambienceController = ambienceController;
        }
        
        public async UniTask Initialize(
            ScenarioState scenarioState,
            string ambienceKey)
        {
            _processor = new DungeonScenarioProcessor(scenarioState, _battleField);
            _ambienceController.SetAmbience(ambienceKey);
            
            _battleField.Initialize(scenarioState.GetTotalCount());
            await _battleField.UpdatePlayerBase();
        }

        public void Reset() => _battleField.ResetSelf();

        void IStartGameListener.OnStartBattle()
        {
            _ambienceController.PlayAmbience();
            AstarPath.active.Scan();
            _processor.IsRunning = true;
            _battleField.OnStartBattle();
        }

        public void Cleanup() => 
            _battleField.Cleanup();

        void IStopGameListener.OnStopBattle()
        {
            _ambienceController.StopAmbience();
            _processor.IsRunning = false;
        }

        void IGameTickable.Tick(float deltaTime)
        {
            if (_processor is not { IsRunning: true }) 
                return;

            if(!_isPaused)
                _processor.Process(deltaTime);

            _battleField.GameUpdate(deltaTime);
        }

        void IGameLateTickable.LateTick(float deltaTime) => 
            _battleField.LateGameUpdate(deltaTime);

        void IPauseListener.SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
            _battleField.SetPaused(isPaused);
        }
    }
}