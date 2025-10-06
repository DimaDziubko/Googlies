using _Game.Core.Configs.Models._GameConfig;
using _Game.Core.UserState._Handler._Analytics;
using _Game.Core.UserState._Handler._BattleSpeed;
using _Game.Core.UserState._Handler._DailyTask;
using _Game.Core.UserState._Handler._Dungeons;
using _Game.Core.UserState._Handler._FoodBoost;
using _Game.Core.UserState._Handler._Purchase;
using _Game.Core.UserState._Handler._Timeline;
using _Game.Core.UserState._Handler._Tutorial;
using _Game.Core.UserState._Handler._Upgrade;
using _Game.Core.UserState._Handler.FreeGemsPack;
using _Game.Core.UserState._State;

namespace _Game.Core.Services.UserContainer
{
    public interface IUserContainer 
    {
        UserAccountState State { get; set; }
        GameConfig GameConfig { get; set; }
        ITimelineStateHandler TimelineStateHandler { get; }
        IAnalyticsStateHandler AnalyticsStateHandler { get; }
        IUpgradeStateHandler UpgradeStateHandler { get; }
        IPurchaseStateHandler PurchaseStateHandler  { get; }
        IFoodBoostStateHandler FoodBoostStateHandler { get; }
        IBattleSpeedStateHandler BattleSpeedStateHandler { get; }
        ITutorialStateHandler TutorialStateHandler { get; }
        IDailyTaskStateHandler DailyTaskStateHandler { get;}
        IAdsGemsPackStateHandler AdsGemsPackStateHandler { get;}
        IFreeGemsPackStateHandler FreeGemsPackStateHandler  { get; }
        IDungeonsStateHandler DungeonsStateHandler  { get; }
        IEventsStateHandler EventsStateHandler { get; }
        void RequestSaveGame(bool isDebounced = false);
    }
}