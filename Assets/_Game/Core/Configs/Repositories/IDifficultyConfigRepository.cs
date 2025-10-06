namespace _Game.Core.Configs.Repositories
{
    public interface IDifficultyConfigRepository
    {
        float GetDifficultyValue(int timeline);
        float GetEvolutionPrice(int timelineId, int ageNumber);
    }
}