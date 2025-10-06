using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay.Vfx.Scripts;
using UnityEngine;
using UnityUtils;

namespace _Game.UI._Currencies
{
    public class CurrencyAnimation : ICurrencyAnimation
    {
        private readonly ICoinFactory _coinFactory;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        public CurrencyAnimation(
            ICoinFactory coinFactory,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;
            _coinFactory = coinFactory;
            _audioService = audioService;
        }
        
        public void PlayCurrency(CurrencyType type, CurrencyVfxRenderMode mode)
        {
            _logger.Log($"PLAY CURRENCY {type}, {mode}", DebugStatus.Warning);
            
            _logger.Log($"THERE IS ATTRACTOR {type}, {mode}", DebugStatus.Warning);
                
            FlyingCurrencyNew currencyVfx = _coinFactory.GetCurrencyVfx(type, mode);

            if (currencyVfx.OrNull() != null)
            {
                currencyVfx.Initialize(Vector2.zero);
                currencyVfx.Launch();
                PlayCoinsSound();
            }
        }
        
        private void PlayCoinsSound() => 
            _audioService.PlayCoinAppearanceSFX();
    }
}