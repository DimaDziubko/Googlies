using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Data;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using Zenject;

namespace _Game.UI._BoostPopup
{
    public class QuickBoostInfoPresenter
    {
        private readonly BoostContainer _boosts;
        private readonly IAudioService _audioService;
        private readonly IBoostPopupProvider _popupProvider;

        private readonly List<QuickBoostDataPresenter> _presenters = new();
        
        private QuickBoostInfoPanel _view;

        private readonly QuickBoostDataPresenter.Factory _factory;
        
        private BoostSource _mainSource;
        private BoostSource _subSource;

        public QuickBoostInfoPresenter(
            QuickBoostInfoPanel view,
            QuickBoostDataPresenter.Factory factory,
            BoostContainer boosts,
            IAudioService audioService,
            IBoostPopupProvider popupProvider)
        {
            _view = view;
            _boosts = boosts;
            _audioService = audioService;
            _popupProvider = popupProvider;
            _factory = factory;
        }

        public void Initialize()
        {
            foreach (var item in _view.Items)
            {
                BoostModel boost = _boosts.GetBoostModel(_view.MainSource, item.BoostType);
                QuickBoostDataPresenter presenter = _factory.Create(boost, item);
                presenter.Initialize();
                _presenters.Add(presenter);
            }
            
            _view.Clicked += OnButtonClicked;
        }

        public void SetView(QuickBoostInfoPanel view) => _view = view;

        public void SetSubSource(BoostSource subSource)
        {
            _subSource = subSource;
        }

        public void SetMainSource(BoostSource mainSource)
        {
            _mainSource = mainSource;
        }

        private async void OnButtonClicked()
        {
            _audioService.PlayButtonSound();
            var popup = await _popupProvider.Load();
            bool isConfirmed = await popup.Value.Show(_view.MainSource, _view.SubSource);
            if (isConfirmed)
            {
                _popupProvider.Dispose();
            }
        }

        public void Dispose()
        {
            _view.Clicked -= OnButtonClicked;
            
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }
        }

        public sealed class Factory : PlaceholderFactory<QuickBoostInfoPanel, QuickBoostInfoPresenter>
        {

        }
    }
}