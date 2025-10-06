using _Game.Core._Logger;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.Utils.Extensions;
using UnityEngine.UIElements;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPointPresenter
    {
        private const string POINT_ACTIVE_STYLE_NAME = "active";
        private const string POINT_CLAIMED_STYLE_NAME = "claimed";
        private const string POINT_LOCKED_STYLE_NAME = "locked";

        
        private readonly RewardPoint _model;
        private readonly VisualElement _view;
        
        private Button _button;
        
        private readonly IMyLogger _logger;

        public WarriorsFundPointPresenter(
            RewardPoint model,
            VisualElement view, 
            IMyLogger logger)
        {
            _model = model;
            _view = view;
            _logger = logger;
        }
        
        public void Initialize()
        {
            var iconHolder = _view.Q<VisualElement>("Reward-icon");
            var label = _view.Q<Label>("Amount-label");
            _button = _view.Q<Button>("Button");

            if (iconHolder != null)
            {
                iconHolder.style.backgroundImage = new StyleBackground(_model.Icon);
            }
            else
            {
                _logger.Log("Warning: Reward-icon-holder is null", DebugStatus.Warning);
            }

            if (label != null)
            {
                label.text = _model.Amount.ToCompactFormat();
            }
            else
            {
                _logger.Log("Warning: Reward-amount-label is null", DebugStatus.Warning);
            }

            if (_button != null)
            {
                CheckButtonState();
                _button.clicked += OnButtonClicked;
            }
            else
            {
                _logger.Log("Warning: Point-btn is null", DebugStatus.Warning);
            }

            _model.OnChangeReady += CheckState;
            _model.OnChangeLocked += CheckState;
            _model.OnChangeClaimed += CheckState;
            
            CheckState();
        }

        private void CheckButtonState()
        {
            _button.SetEnabled(!_model.IsLocked && _model.IsRewardReady && !_model.IsRewardClaimed);
        }

        public void Dispose()
        {
            _button.clicked -= OnButtonClicked;
            
            _model.OnChangeReady -= CheckState;
            _model.OnChangeLocked -= CheckState;
            _model.OnChangeClaimed -= CheckState;
        }

        private void CheckState()
        {
            if (_model.IsLocked)
            {
                _button.SetEnabled(false);
                SetLockedState();
                return;
            }
            
            if(_model.IsRewardClaimed)
            {
                _button.SetEnabled(false);
                SetClaimedState();
                return;
            }
            
            if(_model.IsRewardReady)
            {
                _button.SetEnabled(true);
                SetActiveState();
                return;
            }
            
            SetNormalState();
        }

        private void OnButtonClicked()
        {
            _model.RequestClaimDelayed(0);
            _logger.Log("WF POINT CLICKED", DebugStatus.Info);
        }
        
        private void SetNormalState()
        {
            _view.RemoveFromClassList(POINT_ACTIVE_STYLE_NAME);
            _view.RemoveFromClassList(POINT_CLAIMED_STYLE_NAME);
            _view.RemoveFromClassList(POINT_LOCKED_STYLE_NAME);
        }

        private void SetActiveState()
        {
            _view.RemoveFromClassList(POINT_CLAIMED_STYLE_NAME);
            _view.RemoveFromClassList(POINT_LOCKED_STYLE_NAME);
            _view.AddToClassList(POINT_ACTIVE_STYLE_NAME);
        }

        private void SetClaimedState()
        {
            _view.RemoveFromClassList(POINT_ACTIVE_STYLE_NAME);
            _view.RemoveFromClassList(POINT_LOCKED_STYLE_NAME);
            _view.AddToClassList(POINT_CLAIMED_STYLE_NAME);
        }

        private void SetLockedState()
        {
            _view.RemoveFromClassList(POINT_ACTIVE_STYLE_NAME);
            _view.RemoveFromClassList(POINT_CLAIMED_STYLE_NAME);
            _view.AddToClassList(POINT_LOCKED_STYLE_NAME);
        }
    }
}