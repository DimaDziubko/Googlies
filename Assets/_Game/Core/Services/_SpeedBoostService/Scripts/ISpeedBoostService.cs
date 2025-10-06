using System;
using _Game.UI._Hud._SpeedBoostView.Scripts;

namespace _Game.Core.Services._SpeedBoostService.Scripts
{
    public interface ISpeedBoostService
    {
        event Action<SpeedBoostBtnModel> SpeedBoostBtnModelChanged;
        void OnSpeedBoostBtnShown();
        void OnSpeedBoostBtnClicked();
    }
}