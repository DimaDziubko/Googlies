using System.Collections.Generic;
using _Game.UI._Shop.Scripts._FreeGemsPack;

namespace _Game.Core.Services._FreeGemsPackService
{
    public interface IFreeGemsPackService
    {
        Dictionary<int, FreeGemsPack> GetFreeGemsPacks();
        void OnFreeGemsPackBtnClicked(FreeGemsPack pack);
    }
}