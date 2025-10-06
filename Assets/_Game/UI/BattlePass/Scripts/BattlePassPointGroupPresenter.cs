using _Game.Core._Logger;
using _Game.LiveopsCore.Models.BattlePass;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassPointGroupPresenter
    {
        private const string CONTROL_POINT_ACTIVE_STYLE_NAME = "active";
        public VisualElement View => _view;

        private readonly BattlePassPointGroup _model;
        private readonly VisualElement _view;

        private BattlePassPointPresenter _freePointPresenter;
        private BattlePassPointPresenter _premiumPointPresenter;

        private readonly int _step;

        private readonly IMyLogger _logger;

        private VisualElement _controlPoint;

        private readonly Sprite _pointIcon;

        public BattlePassPointGroupPresenter(
            BattlePassPointGroup model,
            VisualElement view,
            int step,
            Sprite pointIcon,
            IMyLogger logger)
        {
            _pointIcon = pointIcon;
            _logger = logger;
            _model = model;
            _view = view;
            _step = step;
        }

        public void Initialize()
        {
            var freeView = _view.Q<VisualElement>("BP-free-point-container");
            var premiumView = _view.Q<VisualElement>("BP-premium-point-container");

            _controlPoint = _view.Q<VisualElement>("BP-control-point-container");
            
            var controlPointIcon = _view.Q<VisualElement>("BP-control-point-icon");
            
            var label = _view.Q<Label>("BP-control-point-label");

            if (controlPointIcon != null)
            {
                controlPointIcon.style.backgroundImage = new StyleBackground(_pointIcon);
            }
            
            if (label != null)
            {
                label.text = _step.ToString();
            }
            else
            {
                _logger.Log($"BP-free-point-container must have a label.", DebugStatus.Warning);
            }
            
            if (freeView != null)
            {
                _freePointPresenter = new BattlePassPointPresenter(_model.FreePoint, freeView, _logger);
                _freePointPresenter.Initialize();
            }
            else
            {
                _logger.Log("free view is null", DebugStatus.Warning);
            }

            if (premiumView != null)
            {
                _premiumPointPresenter = new BattlePassPointPresenter(_model.PremiumPoint, premiumView, _logger);
                _premiumPointPresenter.Initialize();
            }
            else
            {
                _logger.Log("premium view is null", DebugStatus.Warning);
            }
            
            _model.OnChangeReady += OnReadyChanged;
            OnReadyChanged();
        }

        private void OnReadyChanged()
        {
            if (_controlPoint != null)
            {
                if (_model.IsReady)
                {
                    _logger.Log("Control point add class to class list", DebugStatus.Info);
                    _controlPoint.AddToClassList(CONTROL_POINT_ACTIVE_STYLE_NAME);
                    return;
                }
                
                _controlPoint.RemoveFromClassList(CONTROL_POINT_ACTIVE_STYLE_NAME);
            }
            else
            {
                _logger.Log("control point is null", DebugStatus.Warning);
            }
        }

        public void Dispose()
        {
            _model.OnChangeReady -= OnReadyChanged;
            
            _freePointPresenter?.Dispose();
            _premiumPointPresenter?.Dispose();
        }
    }
}