using System;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.UI._BoostPopup;
using _Game.Utils.Extensions;
using Zenject;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class BoostDataPresenter : IDisposable
    {
        private BoostModel _model;
        private BoostInfoItem _view;
        private IIconConfigRepository _config;
        
        public BoostDataPresenter(
            BoostModel model,
            BoostInfoItem view,
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

        private void OnValueChanged()
        {
            _view.SetName(_model.Type.ToName());
            _view.SetValue($"<style=Golden>x{_model.Value.ToCompactFormat()}</style>");
        }

        public void Dispose()
        {
            _model.Changed -= OnValueChanged;
        }
        
        public sealed class Factory : PlaceholderFactory<BoostModel, BoostInfoItem, BoostDataPresenter>
        {

        }
    }
}