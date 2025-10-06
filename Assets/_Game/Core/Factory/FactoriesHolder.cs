using System.Collections.Generic;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._PickUp._PickUpFactory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;

namespace _Game.Core.Factory
{
    public class FactoriesHolder : IFactoriesHolder
    {
        public IUnitFactory UnitFactory { get; }
        public ICoinFactory CoinFactory { get; }
        public IVfxFactory VfxFactory { get; }
        public IProjectileFactory ProjectileFactory { get; }
        public IBaseFactory BaseFactory { get; }
        public IPickUpFactory PickUpFactory { get; }
        public IEnumerable<GameObjectFactory> Factories { get; }
        
        public FactoriesHolder(
            IUnitFactory unitFactory,
            ICoinFactory coinFactory,
            IVfxFactory vfxFactory,
            IProjectileFactory projectileFactory,
            IBaseFactory baseFactory,
            IPickUpFactory pickUpFactory)
        {
            UnitFactory = unitFactory;
            CoinFactory = coinFactory;
            VfxFactory = vfxFactory;
            ProjectileFactory = projectileFactory;
            BaseFactory = baseFactory;
            PickUpFactory = pickUpFactory;
            
            Factories = new[]
            {
                vfxFactory as GameObjectFactory, 
                unitFactory as GameObjectFactory, 
                projectileFactory as GameObjectFactory, 
                coinFactory as GameObjectFactory, 
                baseFactory as GameObjectFactory, 
                pickUpFactory as GameObjectFactory
            };
        }
    }
}
