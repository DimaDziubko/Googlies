using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public class CurrenciesState : IUserCurrenciesStateReadonly
    {
        public List<CurrencyCellData> Cells;
        
        public bool HasCell(CurrencyType cell) => Cells.Any(x => x.Type == cell);

        public void AddCell(CurrencyCellData cell)
        {
            if (Cells.All(x => x.Type != cell.Type))
            {
                Cells.Add(cell);
            }
        }
    }
}