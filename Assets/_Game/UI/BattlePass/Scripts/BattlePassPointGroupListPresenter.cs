using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.LiveopsCore.Models.BattlePass;
using UnityEngine;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassPointGroupListPresenter
    {
        private readonly IReadOnlyCollection<BattlePassPointGroup> _points;
        private readonly VisualElementListView _listView;
        private readonly IMyLogger _logger;

        private readonly List<BattlePassPointGroupPresenter> _presenters = new();
        
        private readonly Sprite _pointIcon;

        public BattlePassPointGroupListPresenter(
            IReadOnlyCollection<BattlePassPointGroup> points,
            VisualElementListView listView,
            Sprite pointIcon,
            IMyLogger logger)
        {
            _pointIcon = pointIcon;
            _points = points;
            _logger = logger;
            _listView = listView;
        }

        public void Initialize()
        {
            int step = 0;
            
            foreach (var point in _points)
            {
                var presenter = new BattlePassPointGroupPresenter(point, _listView.SpawnElement(), step, _pointIcon, _logger);
                _presenters.Add(presenter);
                presenter.Initialize();
                step++;    
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