using System.Collections.Generic;
using _Game.Core.Boosts;

namespace _Game.UI._BoostPopup
{
    public interface IBoostPopupPresenter
    {
        IEnumerable<BoostModel> GetBoosts(BoostSource source);
    }
}