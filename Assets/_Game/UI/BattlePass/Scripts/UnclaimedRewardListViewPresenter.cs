using System.Collections.Generic;
using _Game.Core._Reward;

namespace _Game.UI.BattlePass.Scripts
{
    public class UnclaimedRewardListViewPresenter
    {
        private readonly VisualElementListView _listView;
        private readonly IEnumerable<IRewardItem> _rewards;
        
        private readonly List<UnclaimedRewardPresenter> _presenters = new();

        public UnclaimedRewardListViewPresenter(VisualElementListView listView, IEnumerable<IRewardItem> rewards)
        {
            _listView = listView;
            _rewards = rewards;
        }

        public void Initialize()
        {
            foreach (var reward in _rewards)
            {
                var presenter = new UnclaimedRewardPresenter(_listView.SpawnElement() ,reward);
                _presenters.Add(presenter);
                presenter.Initialize();
            }
        }

        public void Dispose() => _presenters.Clear();
    }
}