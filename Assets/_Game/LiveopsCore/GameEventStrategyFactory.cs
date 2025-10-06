using _Game.Core._GameEventInfrastructure._GameEventStrategies;
using _Game.Core._GameEventInfrastructure._Trackers;
using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IAP._Processors;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._GameEventRouter;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.Common;
using _Game.LiveopsCore._GameEventStrategies;
using _Game.LiveopsCore._GrantStrategies;
using _Game.LiveopsCore._Trackers;
using _Game.LiveopsCore._UnclaimedRewardShowStrategies;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.LiveopsCore.Models.WarriorsFund;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventStrategyFactory
    {
        private readonly DiContainer _container;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly IUserContainer _userContainer;
        private readonly GameEventRouter _gameEventRouter;
        private readonly ICoroutineRunner _runner;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;
        private readonly IAnalyticsEventSender _sender;
        private readonly IMyLogger _logger;

        public GameEventStrategyFactory(
            DiContainer container,
            RewardProcessingService rewardProcessingService,
            GameEventRouter gameEventRouter,
            IUserContainer userContainer,
            ICoroutineRunner runner,
            IAudioService audioService,
            IIAPService iapService,
            IAnalyticsEventSender sender,
            IMyLogger logger)
        {
            _sender = sender;
            _iapService = iapService;
            _audioService = audioService;
            _runner = runner;
            _gameEventRouter = gameEventRouter;
            _userContainer = userContainer;
            _rewardProcessingService = rewardProcessingService;
            _logger = logger;
            _container = container;
        }

        public IGameEventStrategy GetStrategy(GameEventBase model)
        {
            return model switch
            {
                BattlePassEvent battlePassEvent => new BattlePassStrategy(
                    battlePassEvent,
                    _rewardProcessingService,
                    _logger,
                    _container.Resolve<CurrencyBank>(),
                    _runner,
                    CreateBattlePassPurchaseProcessor(battlePassEvent)
                ),
                ClassicOfferEvent classicOfferEvent => new ClassicOfferStrategy(
                    classicOfferEvent,
                    _logger,
                    CreateClassicOfferPurchaseProcessor(classicOfferEvent)
                ),
                WarriorsFundEvent warriorsFundEvent => new WarriorsFundStrategy(
                    warriorsFundEvent,
                    _rewardProcessingService,
                    _gameEventRouter,
                    _userContainer,
                    CreateWarriorsFundPurchaseProcessor(warriorsFundEvent),
                    _runner,
                    _logger
                ),
                _ => null
            };
        }

        private WarriorsFundPurchaseProcessor CreateWarriorsFundPurchaseProcessor(WarriorsFundEvent warriorsFundEvent)
        {
            return new WarriorsFundPurchaseProcessor(
                _container.Resolve<PurchaseChecker>(),
                warriorsFundEvent,
                _container.Resolve<BalancyPaymentTracker>(),
                _container.Resolve<SaveGameMediator>(),
                _iapService,
                _logger);
        }

        private BattlePassPurchaseProcessor CreateBattlePassPurchaseProcessor(BattlePassEvent battlePassEvent)
        {
            return new BattlePassPurchaseProcessor(
                _container.Resolve<PurchaseChecker>(),
                battlePassEvent,
                _container.Resolve<BalancyPaymentTracker>(),
                _container.Resolve<SaveGameMediator>(),
                _iapService,
                _logger);
        }

        private ClassicOfferPurchaseProcessor CreateClassicOfferPurchaseProcessor(ClassicOfferEvent classicOfferEvent)
        {
            return new ClassicOfferPurchaseProcessor(
                _container.Resolve<PurchaseChecker>(),
                classicOfferEvent,
                _container.Resolve<BalancyPaymentTracker>(),
                _rewardProcessingService,
                _userContainer,
                _runner,
                _container.Resolve<SaveGameMediator>(),
                _iapService,
                _logger);
        }

        public IGameEventUnclaimedRewardGrantStrategy GetPendingRewardGrantStrategy(GameEventBase model)
        {
            return model switch
            {
                BattlePassEvent battlePassEvent => new BattlePassUnclaimedRewardGrantStrategy(
                    battlePassEvent,
                    _rewardProcessingService,
                    _runner),
                WarriorsFundEvent warriorsFundEvent => new WarriorsFundUnclaimedRewardGrantStrategy(
                    warriorsFundEvent,
                    _rewardProcessingService,
                    _runner),
                _ => null,
            };
        }

        public IGameEventUnclaimedRewardShowStrategy GetPendingRewardShowStrategy(GameEventBase model)
        {
            return model switch
            {
                BattlePassEvent battlePassEvent => new BattlePassUnclaimedRewardsShowStrategy(
                    battlePassEvent,
                    _audioService,
                    _logger),
                WarriorsFundEvent warriorsFundEvent => new WarriorsFundUnclaimedRewardsShowStrategy(
                    warriorsFundEvent,
                    _audioService,
                    _logger),
                _ => null
            };
        }

        public IGameEventTracker GetTracker(GameEventBase model)
        {
            return model switch
            {
                BattlePassEvent battlePassEvent => new BattlePassTracker(
                    battlePassEvent,
                    _sender,
                    _userContainer,
                    _logger),
                ClassicOfferEvent classicOfferEvent => new ClassicOfferTracker(
                    classicOfferEvent,
                    _userContainer,
                    _logger,
                    _sender,
                    _container.Resolve<IAPProvider>()
                ),
                WarriorsFundEvent warriorsFundEvent => new WarriorsFundTracker(
                    warriorsFundEvent,
                    _userContainer,
                    _logger,
                    _sender),
                _ => null
            };
        }
    }
}