using _Game.Core._IconContainer;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineInfoItemPresenter
    {
        private TimelineInfoItem _model;
        private AgeInfoView _view;
        
        private readonly AgeIconContainer _container;

        public AgeInfoView View => _view;
        
        public TimelineInfoItemPresenter(
            TimelineInfoItem model, 
            AgeInfoView view
            )
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            _view.SetName(_model.Name);
            _view.SetDescription(_model.Description);

            _view.SetIcon(_model.Icon);
            
            _view.SetDataRange(_model.DateRange);
            _model.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            _view.SetLocked(_model.IsLocked);
            _view.FillMarker(!_model.IsLocked);
        }

        public void Dispose()
        {
            _view.Cleanup();
            _model.StateChanged -= OnStateChanged;
        }

        public void PlayRippleAnimation(in float duration)
        {
            _view.PlayRippleAnimation(duration);
        }

        public void SetLocked(bool isLocked) => _view.SetLocked(isLocked);
    }
}