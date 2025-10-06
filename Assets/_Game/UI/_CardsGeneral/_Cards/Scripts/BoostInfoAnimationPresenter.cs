using System;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils.Extensions;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class BoostInfoAnimationPresenter : IDisposable
    {
        private BoostModel _model;
        private BoostInfoAnimationView _view;
        private IIconConfigRepository _config;
        
        private float _currentValue;
        private float _delta;

        public BoostInfoAnimationPresenter(
            BoostModel model,
            BoostInfoAnimationView view,
            IIconConfigRepository config)
        {
            _model = model;
            _view = view;
            _config = config;
        }

        public void Initialize()
        {
            BoostType type = _model.Type;
            _view.SetIcon(_config.ForBoostIcon(type));
            _model.Changed += OnValueChanged;
            _currentValue = _model.Value;
            
            _view.Disable();
        }

        private void OnValueChanged()
        {
            _delta = _model.Value - _currentValue;
            _currentValue = _model.Value;
            
            _view.SetValue(_currentValue.ToCompactFormat());
            _view.SetDeltaValue(_delta.ToCompactFormat());

            _view.Cleanup();
            _view.PlayFade();
        }

        public void Dispose()
        {
            _view.Cleanup();
            _model.Changed -= OnValueChanged;
        }
    }
}