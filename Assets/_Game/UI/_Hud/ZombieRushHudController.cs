using _Game.Core._Logger;
using _Game.Core.Services._Camera;

namespace _Game.UI._Hud
{
    public class ZombieRushHudController
    {
        public ZombieRushHudController(
            IWorldCameraService cameraService, 
            ZombieRushHud zombieRushHud,
            IMyLogger logger)
        {
            zombieRushHud.Construct(cameraService, logger);
        }
    }
}