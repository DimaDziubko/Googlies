namespace _Game.Core.Configs.Repositories
{
    public interface IFeatureConfigRepository
    {
        bool IsDungeonsUnlocked { get; }
        int SkillRequiredTimeline { get;}
        bool IsSkillsUnlocked { get;}
    }
}