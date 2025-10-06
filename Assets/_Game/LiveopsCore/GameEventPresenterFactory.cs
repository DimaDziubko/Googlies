using System;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using _Game.LiveopsCore._EventPresenter;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.UI._ParticleAttractorSystem;

namespace _Game.LiveopsCore
{
    public class GameEventPresenterFactory
    {
        private readonly IMyLogger _logger;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly CurrencyBank _bank;
        private readonly IUserContainer _userContainer;
        private readonly IIAPService _iapService;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly IConfigRepository _config;
        private readonly IAdsService _adsService;
        private readonly IParticleAttractorRegistry _particleAttractorRegistry;
        private readonly IAPProvider _provider;

        public GameEventPresenterFactory(
            IMyLogger logger,
            IAudioService audioService,
            IWorldCameraService cameraService,
            CurrencyBank bank,
            IUserContainer userContainer,
            IIAPService iapService,
            RewardProcessingService rewardProcessingService,
            IConfigRepository config,
            IParticleAttractorRegistry particleAttractorRegistry,
            IAdsService adsService, 
            IAPProvider provider)
        {
            _logger = logger;
            _audioService = audioService;
            _cameraService = cameraService;
            _bank = bank;
            _userContainer = userContainer;
            _iapService = iapService;
            _rewardProcessingService = rewardProcessingService;
            _config = config;
            _particleAttractorRegistry = particleAttractorRegistry;
            _adsService = adsService;
            _provider = provider;
        }

        public GameEventPresenterBase CreatePresenter(GameEventBase gameEvent, GameEventView view)
        {
            return gameEvent switch
            {
                BattlePassEvent battlePassEvent => new BattlePassPresenter(battlePassEvent, view, _logger, _cameraService, _audioService, _bank, _userContainer, _config.IconConfigRepository, _particleAttractorRegistry, _provider),
                WarriorsFundEvent warriorsFundEvent => new WarriorsFundPresenter(warriorsFundEvent, view, _logger, _cameraService, _audioService, _bank, _userContainer, _config.IconConfigRepository, _particleAttractorRegistry, _provider),
                ClassicOfferEvent classicOfferEvent => new ClassicOfferPresenter(classicOfferEvent, view, _logger, _cameraService, _audioService, _bank, _userContainer, _config.IconConfigRepository, _particleAttractorRegistry, _provider),
                _=> null
            };
        }
    }
}