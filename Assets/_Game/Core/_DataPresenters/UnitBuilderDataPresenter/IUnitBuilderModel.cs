using System.Collections.Generic;
using _Game.Gameplay._UnitBuilder.Scripts;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public interface IUnitBuilderModel
    {
        IEnumerable<UnitBuilderBtnModel> GetBtnModels();
    }
}