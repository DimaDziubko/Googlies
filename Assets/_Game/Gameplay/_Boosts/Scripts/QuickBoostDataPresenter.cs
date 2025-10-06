using System;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.UI._BoostPopup;
using _Game.Utils.Extensions;
using Zenject;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class QuickBoostDataPresenter : IDisposable
    {
        private BoostModel _model;
        private QuickBoostInfoItem _view;
        private IIconConfigRepository _config;
        
        public QuickBoostDataPresenter(
            BoostModel model,
            QuickBoostInfoItem view,
            IConfigRepository configRepositoryFacade)
        {
            _model = model;
            _view = view;
            _config = configRepositoryFacade.IconConfigRepository;
        }

        public void Initialize()
        {
            var type = _view.BoostType == BoostType.None ? _model.Type : _view.BoostType;
            _view.SetIcon(_config.ForBoostIcon(type));
            _model.Changed += OnValueChanged;
            OnValueChanged();
        }

        private void OnValueChanged() => 
            _view.SetValue($"x{_model.Value.ToCompactFormat()}");

        public void Dispose()
        {
            _model.Changed -= OnValueChanged;
        }
        
        public sealed class Factory : PlaceholderFactory<BoostModel, QuickBoostInfoItem, QuickBoostDataPresenter>
        {

        }
    }
}