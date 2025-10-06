using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Data;

namespace _Game.UI._BoostPopup
{
    public class BoostPopupPresenter : IBoostPopupPresenter
    {
        private readonly BoostContainer _boosts;

        public BoostPopupPresenter(BoostContainer boosts)
        {
            _boosts = boosts;
        }

        public IEnumerable<BoostModel> GetBoosts(BoostSource source) => 
            _boosts.GetBoostModels(source);
    }
}