using System;
using System.Collections.Generic;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._Header.Scripts;
using _Game.UI._ParticleAttractorSystem;
using Zenject;

namespace _Game.UI._Currencies
{
    public class CoinsAndGemsPresenter : IInitializable, IDisposable
    {
        private readonly CurrencyBank _bank;
        private readonly Header _header;
        private readonly IConfigRepository _config;
        private readonly IParticleAttractorRegistry _registry;
        private readonly IAudioService _audioService;

        private readonly List<CurrencyCellPresenter> _presenters = new();

        public CoinsAndGemsPresenter(
            CurrencyBank bank, 
            Header header, 
            IConfigRepository config,
            IParticleAttractorRegistry registry,
            IAudioService audioService)
        {
            _bank = bank;
            _header = header;
            _config = config;
            _registry = registry;
            _audioService = audioService;
        }
        
        public void Initialize()
        {
            CurrencyCell coinCell = _bank.GetCell(CurrencyType.Coins);
            CurrencyView coinsView = _header.CurrenciesUI.CoinsView;
            CurrencyCellPresenter coinsPresenter = new CurrencyCellPresenter(coinCell, coinsView, _config.IconConfigRepository, _registry, _audioService);
            coinsPresenter.Initialize();
            _presenters.Add(coinsPresenter);
            
            CurrencyCell gemsCell = _bank.GetCell(CurrencyType.Gems);
            CurrencyView gemsView = _header.CurrenciesUI.GemsView;
            CurrencyCellPresenter gemsPresenter = new CurrencyCellPresenter(gemsCell, gemsView, _config.IconConfigRepository, _registry, _audioService);
            gemsPresenter.Initialize();
            _presenters.Add(gemsPresenter);
        }

        public void Dispose()
        {
            foreach (CurrencyCellPresenter presenter in _presenters)
            {
                presenter.Dispose();
            }
        }
    }
}