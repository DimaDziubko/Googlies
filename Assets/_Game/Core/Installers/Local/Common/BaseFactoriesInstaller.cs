using System;
using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core.Factory;
using _Game.Core.Loading;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Random;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._PickUp._PickUpFactory;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI._Environment.Factory;
using _Game.UI._ParticleAttractorSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Local.Common
{
    public abstract class BaseFactoriesInstaller : MonoInstaller
    {
        [SerializeField, Required] protected UnitFactory _unitFactory;
        [SerializeField, Required] protected BaseFactory baseFactory;
        [SerializeField, Required] protected ProjectileFactory _projectileFactory;
        [SerializeField, Required] protected CoinFactory _coinFactory;
        [SerializeField, Required] protected VfxFactory _vfxFactory;
        [SerializeField, Required] protected EnvironmentFactory _environmentFactory;
        [SerializeField, Required] protected PickUpFactory _pickUpFactory;

        public override void InstallBindings()
        {
            BindFactories();
            BindFactoriesHolder();
        }

        protected abstract IUnitDataProvider GetUnitDataProvider();
        protected abstract IWeaponDataProvider GetWeaponDataProvider();
        protected abstract IBaseDataProvider GetBaseDataProvider();

        private void BindFactories()
        {
            var unitDataProvider = GetUnitDataProvider();
            var weaponDataProvider = GetWeaponDataProvider();
            var baseDataProvider = GetBaseDataProvider();

            var cameraService = Container.Resolve<IWorldCameraService>();
            var random = Container.Resolve<IRandomService>();
            var audioService = Container.Resolve<IAudioService>();
            var soundService = Container.Resolve<ISoundService>();
            var logger = Container.Resolve<IMyLogger>();
            var targetRegistry = Container.Resolve<ITargetRegistry>();
            var environmentFactoryMediator = Container.Resolve<EnvironmentFactoryMediator>();
            var baseFactoryMediator = Container.Resolve<BaseFactoryMediator>();
            var particleRegistry = Container.Resolve<IParticleAttractorRegistry>();

            BindFactory(_unitFactory,
                f => f.Initialize(cameraService, random, soundService, unitDataProvider, targetRegistry, logger),
                typeof(IUnitFactory));
            BindFactory(_projectileFactory, f => f.Initialize(targetRegistry, soundService, weaponDataProvider),
                typeof(IProjectileFactory));
            BindFactory(baseFactory,
                f => f.Initialize(targetRegistry, baseDataProvider, cameraService, logger, baseFactoryMediator),
                typeof(IBaseFactory));
            BindFactory(_coinFactory, f => f.Construct(audioService, cameraService, particleRegistry, logger),
                typeof(ICoinFactory));
            BindFactory(_vfxFactory,
                f => f.Initialize(targetRegistry, cameraService, audioService, logger),
                typeof(IVfxFactory));
            BindEnvironmentFactory(cameraService, environmentFactoryMediator);
            BindFactory(_pickUpFactory, f => f.Construct(audioService), typeof(IPickUpFactory));
        }

        private void BindFactory<TConcrete>(TConcrete factory, Action<TConcrete> init, Type bindType)
            where TConcrete : ScriptableObject
        {
            init(factory);
            Container.Bind(bindType).To(factory.GetType()).FromScriptableObject(factory).AsSingle();
        }

        private void BindEnvironmentFactory(IWorldCameraService cameraService, EnvironmentFactoryMediator mediator)
        {
            _environmentFactory.Initialize(cameraService, mediator);
            Container
                .Bind<IEnvironmentFactory>()
                .To<EnvironmentFactory>()
                .FromInstance(_environmentFactory)
                .AsSingle();
        }

        private void BindFactoriesHolder() => Container.BindInterfacesAndSelfTo<FactoriesHolder>().AsSingle();
    }
}