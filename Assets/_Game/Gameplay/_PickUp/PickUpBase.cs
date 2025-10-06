using _Game.Gameplay._PickUp._PickUpFactory;
using _Game.Gameplay.Common;

namespace _Game.Gameplay._PickUp
{
    public abstract class PickUpBase : GameBehaviour
    {
        public IPickUpFactory OriginFactory { get; set; }

        public PowerUpType Type { get; set; }

        public override void Recycle()
        {
            OriginFactory.Reclaim(Type, this);
        }
    }
}