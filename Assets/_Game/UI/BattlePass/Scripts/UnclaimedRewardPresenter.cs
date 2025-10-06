using System.Globalization;
using _Game.Core._Reward;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class UnclaimedRewardPresenter
    {
        private readonly VisualElement _view;
        private readonly IRewardItem _model;
        
        public UnclaimedRewardPresenter(
            VisualElement view,
            IRewardItem model)
        {
            _view = view;
            _model = model;
        }

        public void Initialize()
        {
            var iconHolder = _view.Q<VisualElement>("Reward-icon");
            var label = _view.Q<Label>("Amount-label");
            
            iconHolder.style.backgroundImage = new StyleBackground(_model.Icon);
            label.text = _model.Amount.ToString(CultureInfo.InvariantCulture);
        }
    }
}