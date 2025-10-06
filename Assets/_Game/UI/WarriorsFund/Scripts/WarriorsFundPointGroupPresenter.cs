using _Game.Core._Logger;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.WarriorsFund;
using UnityEngine.UIElements;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPointGroupPresenter
    {
        private const string CONTROL_POINT_ACTIVE_STYLE_NAME = "active";
        
        private readonly WarriorsFundPointGroup _model;
        private readonly IMyLogger _logger;
        public VisualElement View { get; private set; }

        private WarriorsFundPointPresenter _freePointPresenter;
        private WarriorsFundPointPresenter _premiumPointPresenter;
        
        private VisualElement _unitIconHolder;
        private VisualElement _controlPointIcon;
        private Label _objectiveLabel;

        public WarriorsFundPointGroupPresenter(
            WarriorsFundPointGroup model,
            VisualElement view,
            IMyLogger logger)
        {
            View = view;
            _model = model;
            _logger = logger;
        }

        public void Initialize()
        {
            VisualElement freeView = View.Q<VisualElement>("Free-reward-container");
            VisualElement premiumView = View.Q<VisualElement>("Premium-reward-container");
            
            _unitIconHolder = View.Q<VisualElement>("Unit-icon-holder");
            _controlPointIcon = View.Q<VisualElement>("Control-point-icon");
            
            _objectiveLabel = View.Q<Label>("Condition-text-label");
            
            if (freeView != null)
            {
                _freePointPresenter = new WarriorsFundPointPresenter(_model.FreePoint, freeView, _logger);
                _freePointPresenter.Initialize();
            }
            else
            {
                _logger.Log("Free view is null", DebugStatus.Warning);
            }

            if (premiumView != null)
            {
                _premiumPointPresenter = new WarriorsFundPointPresenter(_model.PremiumPoint, premiumView, _logger);
                _premiumPointPresenter.Initialize();
            }
            else
            {
                _logger.Log("Premium view is null", DebugStatus.Warning);
            }

            if (_objectiveLabel != null)
            {
                UserProgress objective = _model.Objective;
                string objectiveText = objective.TimelineNumber == 1 
                    ? $"Age {objective.AgeNumber}" 
                    : $"Timeline {objective.TimelineNumber}\n Age {objective.AgeNumber}";
                _objectiveLabel.text = objectiveText;
            }
            else
            {
                _logger.Log("Objective label is null", DebugStatus.Warning);
            }
            
            _model.OnChangeReady += OnReadyChanged;
            
            OnReadyChanged();
        }

        public void Dispose()
        {
            _model.OnChangeReady -= OnReadyChanged;
            
            _freePointPresenter?.Dispose();
            _premiumPointPresenter?.Dispose();
        }

        private void OnReadyChanged()
        {
            if(_unitIconHolder != null)
                _unitIconHolder.style.backgroundImage = new StyleBackground(_model.Icon);
            
            if (_controlPointIcon != null)
            {
                if (_model.IsReady)
                {
                    _logger.Log("Control point add class to class list", DebugStatus.Info);
                    _controlPointIcon.AddToClassList(CONTROL_POINT_ACTIVE_STYLE_NAME);
                    return;
                }
                
                _controlPointIcon.RemoveFromClassList(CONTROL_POINT_ACTIVE_STYLE_NAME);
            }
        }
    }
}