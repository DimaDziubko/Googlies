using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public class FeatureConfigRepository : IFeatureConfigRepository   
    {
        private readonly IUserContainer _userContainer;

        public FeatureConfigRepository(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public bool IsDungeonsUnlocked => _userContainer.GameConfig.FeatureSettings.IsDungeonsUnlocked;

        public int SkillRequiredTimeline => _userContainer.GameConfig.SkillExtraConfig.RequiredTimeline;

        public bool IsSkillsUnlocked => _userContainer.GameConfig.FeatureSettings.IsSkillsUnlocked;
    }
}