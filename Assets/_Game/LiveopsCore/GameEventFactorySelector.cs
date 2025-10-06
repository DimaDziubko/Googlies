using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Loading;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardItemResolver;
using _Game.LiveopsCore.AbstractFactory;
using _Game.LiveopsCore.ConcreteFactory.BattlePassFactory;
using _Game.LiveopsCore.ConcreteFactory.WarriorsFundFactory;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.Utils.Extensions;
using Balancy.Data.SmartObjects;
using Balancy.Models;

namespace _Game.LiveopsCore
{
    public class GameEventFactorySelector
    {
        private readonly Dictionary<GameEventType, IGameEventFactory> _factories;
        private readonly IMyLogger _logger;
        private readonly IConfigRepository _config;
        private readonly RewardItemResolver _resolver;
        private readonly IUserContainer _userContainer;

        public GameEventFactorySelector(
            IBalancySDKService balancy,
            IUserContainer container,
            IConfigRepository config,
            IMyLogger logger,
            RewardItemResolver resolver,
            IUserContainer userContainer,
            LoadingOperationFactory loadingOperationFactory,
            IGameInitializer gameInitializer
        )
        {
            _logger = logger;
            _config = config;
            _resolver = resolver;
            _userContainer = userContainer;

            _factories = new Dictionary<GameEventType, IGameEventFactory>
            {
                { GameEventType.BattlePass, new BattlePassEventFactory(_userContainer, _logger, config.IconConfigRepository, _resolver, balancy) },
                { GameEventType.WarriorsFund, new WarriorsFundEventFactory(_userContainer, _logger, config.IconConfigRepository, _resolver, balancy) },
                { GameEventType.ClassicOffer, new ClassicOfferEventFactory(_userContainer, _logger, config.IconConfigRepository, _resolver, balancy) },
            };
        }

        public GameEventBase TryCreateWithPendingRewards(GameEventSavegame save)
        {
            if (!_factories.TryGetValue(save.EventType, out var factory))
            {
                _logger.LogError($"No factory registered for event type {save.EventType}");
                return null;
            }

            return factory.TryCreateWithPendingRewards(save);
        }

        public GameEventBase Create(GameEventSavegame save)
        {
            if (!_factories.TryGetValue(save.EventType, out var factory))
            {
                _logger.LogError($"No factory registered for event type {save.EventType}");
                return null;
            }

            return factory.CreateModel(save);
        }

        public GameEventBase Create(EventInfo remote, GameEventSavegame save)
        {
            if (remote.GameEvent is not CustomGameEvent custom)
            {
                _logger.LogError("Remote event must be a CustomGameEvent");
                return null;
            }

            var concrete = custom.ConcreteEvent;

            if (concrete == null)
            {
                _logger.LogError("Concrete event is null");
                return null;
            }

            GameEventType type = concrete.GameEventType.ToLocal();

            if (!_factories.TryGetValue(type, out var factory))
            {
                _logger.LogError($"No factory registered for type {type}");
                return null;
            }

            return factory.CreateModel(remote, save, isNew: false);
        }

        public GameEventBase Create(EventInfo remote)
        {
            if (remote.GameEvent is not CustomGameEvent custom)
            {
                _logger.LogError("Remote event must be a CustomGameEvent");
                return null;
            }

            var concrete = custom.ConcreteEvent;

            if (concrete == null)
            {
                _logger.LogError("Concrete event is null");
                return null;
            }

            GameEventType type = concrete.GameEventType.ToLocal();

            if (!_factories.TryGetValue(type, out var factory))
            {
                _logger.LogError($"No factory registered for type {type}");
                return null;
            }

            return factory.CreateModel(remote, null, isNew: true);
        }
    }
}