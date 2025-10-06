using System.Collections.Generic;
using _Game._BattleModes.Scripts;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using Zenject;

namespace _Game.Core.Loading
{
    public class LoadingOperationFactory
    {
        private readonly IUserContainer _userContainer;
        private readonly EnvironmentFactoryMediator _environmentFactoryMediator;
        private readonly ForcedUpdateService _forcedUpdateService;
        private readonly BaseFactoryMediator _baseFactoryMediator;

        private readonly WarriorIconOperationFactory _warriorIconOperationFactory;
        private readonly AgeIconOperationFactory _ageIconOperationFactory;
        private readonly AmbienceOperationFactory _ambienceOperationFactory;
        private readonly ShopIconOperationFactory _shopIconsOperationFactory;

        private readonly GameModeOperationFactory _gameModeOperationFactory;

        private readonly IMyLogger _logger;

        public LoadingOperationFactory(
            DiContainer container,
            IMyLogger logger)
        {
            _logger = logger;
    
            IUserContainer userContainer = container.Resolve<IUserContainer>();
            IConfigRepository config = container.Resolve<IConfigRepository>();
            IAssetRegistry assetRegistry = container.Resolve<IAssetRegistry>();
            IWorldCameraService cameraService = container.Resolve<IWorldCameraService>();
            ISoundService soundService = container.Resolve<ISoundService>();
            
            _userContainer = userContainer;
            
            WarriorIconContainer warriorIconContainer = container.Resolve<WarriorIconContainer>();
            AgeIconContainer ageIconContainer = container.Resolve<AgeIconContainer>();
            AmbienceContainer ambienceContainer = container.Resolve<AmbienceContainer>();
            ShopIconsContainer shopIconsContainer = container.Resolve<ShopIconsContainer>();
            
            SceneLoader sceneLoader = container.Resolve<SceneLoader>();
            
            _environmentFactoryMediator = container.Resolve<EnvironmentFactoryMediator>();
            _forcedUpdateService = container.Resolve<ForcedUpdateService>();
            _baseFactoryMediator = container.Resolve<BaseFactoryMediator>();

            _warriorIconOperationFactory = new WarriorIconOperationFactory(userContainer, config, assetRegistry, warriorIconContainer, logger);
            _ageIconOperationFactory = new AgeIconOperationFactory(userContainer, config, assetRegistry, ageIconContainer, logger);
            _ambienceOperationFactory = new AmbienceOperationFactory(userContainer, config, assetRegistry, ambienceContainer, logger);
            _shopIconsOperationFactory = new ShopIconOperationFactory(config, assetRegistry, shopIconsContainer, logger);
            _gameModeOperationFactory = new GameModeOperationFactory(sceneLoader, cameraService, soundService, logger);
        }

        public ILoadingOperation CreateBattleModeLoadingOperation() => 
            _gameModeOperationFactory.CreateBattleModeLoadingOperation();

        public ILoadingOperation CreateClearModeOperation(IGameModeCleaner cleaner) => 
            _gameModeOperationFactory.CreateClearModeOperation(cleaner);

        public ILoadingOperation CreateUnloadGameModeOperation(string battleMode) => 
            _gameModeOperationFactory.CreateUnloadGameModeOperation(battleMode);

        public ILoadingOperation CreateZombieRushModeLoadingOperation(IDungeonModel model) => 
            _gameModeOperationFactory.CreateZombieRushModeLoadingOperation(model);


        public ILoadingOperation CreateBattleTransitionOperation(IBattleNavigator battleNavigator) =>
            new BattleTransitionOperation(battleNavigator);

        public ILoadingOperation CreateInitialAgeIconsLoadingOperation() => 
            _ageIconOperationFactory.CreateInitialAgeIconsLoadingOperation();

        public ILoadingOperation CreateAgeIconsLoadingOperation(List<IIconReference> iconsToLoad) => 
            _ageIconOperationFactory.CreateAgeIconsLoadingOperation(iconsToLoad);

        public ILoadingOperation CreateAgeIconsReleasingOperation(List<IIconReference> iconsToRelease) => 
            _ageIconOperationFactory.CreateAgeIconsReleasingOperation(iconsToRelease);


        public ILoadingOperation CreateInitialAmbienceLoadingOperation() => 
            _ambienceOperationFactory.CreateInitialAmbienceLoadingOperation();

        public ILoadingOperation CreateAmbienceLoadingOperation(List<string> ambienceToLoad) => 
            _ambienceOperationFactory.CreateAmbienceLoadingOperation(ambienceToLoad);

        public ILoadingOperation CreateAmbienceReleasingOperation(List<string> ambienceToRelease) => 
            _ambienceOperationFactory.CreateAmbienceReleasingOperation(ambienceToRelease);


        public ILoadingOperation CreateInitialWarriorIconsLoadingOperation() => 
            _warriorIconOperationFactory.CreateInitialWarriorIconsLoadingOperation();
        public ILoadingOperation CreateWarriorIconsLoadingOperation(IEnumerable<IIconReference> iconsToLoad) => 
            _warriorIconOperationFactory.CreateWarriorIconsLoadingOperation(iconsToLoad);
        public ILoadingOperation CreateWarriorIconsReleasingOperation(IEnumerable<IIconReference> iconsToRelease) =>
            _warriorIconOperationFactory.CreateWarriorIconsReleasingOperation(iconsToRelease);
        public ILoadingOperation CreateAgeWarriorIconsLoadingOperation(int timelineId, int ageId) => 
            _warriorIconOperationFactory.CreateAgeWarriorIconsLoadingOperation(timelineId, ageId);
        public ILoadingOperation CreateAgeWarriorIconsReleasingOperation(int timelineId, int ageId) => 
            _warriorIconOperationFactory.CreateAgeWarriorIconsReleasingOperation(timelineId, ageId);

        public ILoadingOperation CreateResetOperation(IResetable resetable) =>
            new ResetOperation(resetable);

        public ILoadingOperation CreateConfigOperation() =>
            new ConfigOperation(_userContainer, _logger, _forcedUpdateService);

        public ILoadingOperation CreateDungeonsValidationOperation() =>
            new DungeonsValidationOperation(_userContainer, _logger);
        
        public ILoadingOperation CreateEnvironmentClearingOperation() =>
            new EnvironmentClearingOperation(_environmentFactoryMediator, _logger);
        
        public ILoadingOperation CreateBaseClearingOperation() =>
            new BaseClearingOperation(_baseFactoryMediator, _logger);

        public ILoadingOperation CreateShopIconsLoadingOperation() => 
            _shopIconsOperationFactory.CreateShopIconsLoadingOperation();
        public ILoadingOperation ShopIconsReleasingOperation() => 
            _shopIconsOperationFactory.ShopIconsReleasingOperation();

        public ILoadingOperation CreateParallelOperation(string description, Queue<ILoadingOperation> parallelOperations) => 
            new ParallelOperation(description, parallelOperations);
    }
}