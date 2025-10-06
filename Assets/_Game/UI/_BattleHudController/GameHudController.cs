using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.UI._Hud;
using Zenject;

namespace _Game.UI._BattleHudController
{
    public class GameHudController :
        IInitializable,
        IStopGameListener
    {
        private readonly BattleHud _battleHud;
        
        public GameHudController(
            BattleHud battleHud,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {

            _battleHud = battleHud;
            
            _battleHud.Construct(
                cameraService,
                logger);
        }

        void IInitializable.Initialize() => _battleHud.Init();
        void IStopGameListener.OnStopBattle() => _battleHud.WaveInfoPopup.HideWave();
    }
}