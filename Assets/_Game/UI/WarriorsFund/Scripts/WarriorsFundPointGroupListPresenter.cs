using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.UI.BattlePass.Scripts;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPointGroupListPresenter
    {
        private readonly IReadOnlyCollection<WarriorsFundPointGroup> _points;
        private readonly VisualElementListView _listView;
        private readonly IMyLogger _logger;

        private readonly List<WarriorsFundPointGroupPresenter> _presenters = new();

        public WarriorsFundPointGroupListPresenter(
            IReadOnlyCollection<WarriorsFundPointGroup> points,
            VisualElementListView listView,
            IMyLogger logger)
        {
            _points = points;
            _logger = logger;
            _listView = listView;
        }

        public void Initialize()
        {
            foreach (var point in _points)
            {
                var presenter = new WarriorsFundPointGroupPresenter(point, _listView.SpawnElement(), _logger);
                _presenters.Add(presenter);
                presenter.Initialize();
            }
        }

        public void Dispose()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
                _listView.DestroyElement(presenter.View);
            }

            _presenters.Clear();
            _listView.Clear();
        }
    }
}