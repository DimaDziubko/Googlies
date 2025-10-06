using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay.Common;
using _Game.Gameplay.Vfx.Factory;
using Sirenix.OdinInspector;

namespace _Game.Gameplay.Vfx.Scripts
{
    public abstract class VfxEntity : GameBehaviour
    {
        [ShowInInspector, ReadOnly]
        protected IWorldCameraService _cameraService;
        [ShowInInspector, ReadOnly]
        protected ITargetRegistry _targetRegistry;
        [ShowInInspector, ReadOnly]
        protected IAudioService _audioService;

        public void Construct(
            IWorldCameraService cameraService,
            ITargetRegistry targetRegistry,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _targetRegistry = targetRegistry;
            _audioService = audioService;
        }
        public IVfxFactory OriginFactory { get; set; }
        public override void Recycle() => OriginFactory.Reclaim(this);
    }
    
    public abstract class KeyedVfxEntity : VfxEntity
    {
        public string Key { get; set; }
        public void SetKey(string key) => Key = key;
        public override void Recycle() => OriginFactory.Reclaim(Key, this);
    }
}