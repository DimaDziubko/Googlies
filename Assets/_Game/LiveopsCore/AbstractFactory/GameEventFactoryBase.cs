using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardItemResolver;
using Balancy.Data.SmartObjects;

namespace _Game.LiveopsCore.AbstractFactory
{
    public abstract class GameEventFactoryBase : IGameEventFactory
    {
        protected readonly IUserContainer _userContainer;
        protected readonly IMyLogger _logger;
        protected readonly IIconConfigRepository _iconConfig;
        protected readonly RewardItemResolver _rewardItemResolver;
        protected readonly IBalancySDKService _balancy;

        protected ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        protected GameEventFactoryBase(
            IUserContainer userContainer,
            IMyLogger logger,
            IIconConfigRepository iconConfig,
            RewardItemResolver rewardItemResolver,
            IBalancySDKService balancy)
        {
            _balancy = balancy;
            _logger = logger;
            _userContainer = userContainer;
            _rewardItemResolver = rewardItemResolver;
            _iconConfig = iconConfig;
        }

        public abstract GameEventBase CreateModel(EventInfo remote, GameEventSavegame save, bool isNew);
        public abstract GameEventBase CreateModel(GameEventSavegame save);
        public abstract GameEventBase TryCreateWithPendingRewards(GameEventSavegame save);
    }
}