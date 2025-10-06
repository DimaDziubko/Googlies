using System;
using _Game.UI._Hud._BattleSpeedView;

namespace _Game.Core.Services._BattleSpeedService._Scripts
{
    public interface IBattleSpeedService
    {
        event Action<BattleSpeedBtnModel> BattleSpeedBtnModelChanged;
        void OnBattleSpeedBtnClicked();
        void OnBattleSpeedBtnShown();
    }
}