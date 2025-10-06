using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._ParticleAttractorSystem;
using _Game.Utils.Extensions;

namespace _Game.UI._Currencies
{
    public sealed class CurrencyCellPresenter
    {
        public CurrencyView View => _view;
        public CurrencyCell Cell => _cell;

        private CurrencyCell _cell;
        private CurrencyView _view;
        
        private readonly IIconConfigRepository _config;
        private readonly IParticleAttractorRegistry _registry;
        private readonly IAudioService _audioService;

        public CurrencyCellPresenter(
            CurrencyCell cell, 
            CurrencyView view,
            IIconConfigRepository config,
            IParticleAttractorRegistry registry,
            IAudioService audioService)
        {
            _audioService = audioService;
            _registry = registry;
            _cell = cell;
            _view = view;
            _config = config;
        }

        public void Initialize()
        {
            _registry.Register(_view.Attractor.ParticleAttractableType, _view.Attractor.Attractor);
            _view.Attractor.Attractor.onAttracted.AddListener(OnAttracted);
            
            var icon = _config.GetCurrencyIconFor(_cell.Type);
            _view.SetIcon(icon);
            _view.SetupCurrency(_cell.Amount.ToCompactFormat(GetThreshold()));
            
            _cell.OnAmountAdded += OnAmountAdded;
            _cell.OnAmountSpent += OnAmountSpent;
            _cell.OnAmountChanged += OnAmountChanged;
        }

        public void Dispose()
        {
            _view.Attractor.Attractor.onAttracted.RemoveListener(OnAttracted);
            _registry.TryDeregister(_view.Attractor.ParticleAttractableType);

            _view.Cleanup();
            
            _cell.OnAmountAdded -= OnAmountAdded;
            _cell.OnAmountSpent -= OnAmountSpent;
            _cell.OnAmountChanged -= OnAmountChanged;
        }

        private void OnAttracted() => 
            _audioService.PlayVfxAttractSound(_view.Attractor.ParticleAttractableType);

        public void SetData(CurrencyCell cell, CurrencyView view)
        {
            _view = view;
            _cell = cell;
        }

        private void OnAmountChanged(double currency)
        {
            _view.ChangeCurrency(currency.ToCompactFormat(GetThreshold()));
        }

        private void OnAmountAdded(double range)
        {
            double amount = _cell.Amount;
            _view.AddCurrency((amount - range), range, GetThreshold());
        }

        private void OnAmountSpent(double _)
        {
            double amount = _cell.Amount;
            _view.RemoveCurrency( amount.ToCompactFormat(GetThreshold()));
        }
        
        private int GetThreshold()
        {
            return Cell.Type switch
            {
                CurrencyType.Gems => 9999,
                _ => 999
            };
        }
    }
}