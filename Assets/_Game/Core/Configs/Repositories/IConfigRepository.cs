using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.Analytics;

namespace _Game.Core.Configs.Repositories
{
    public interface IConfigRepository
    {
        ICardsConfigRepository CardsConfigRepository { get; }
        ITimelineConfigRepository TimelineConfigRepository { get; }
        IIconConfigRepository IconConfigRepository { get; }
        IBattleSpeedConfigRepository BattleSpeedConfigRepository { get; }
        IEconomyConfigRepository EconomyConfigRepository { get; }
        IDailyTaskConfigRepository DailyTaskConfigRepository { get; }
        IShopConfigRepository ShopConfigRepository { get; }
        IDifficultyConfigRepository DifficultyConfigRepository { get;}
        IAdsConfigRepository AdsConfigRepository { get; }
        IFeatureConfigRepository  FeatureConfigRepository { get;}
        ISkillConfigRepository  SkillConfigRepository { get;}
        FBConfigIDTO GetFBConfigIds();
    }
}