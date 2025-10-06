namespace _Game.Core._GameListenerComposite
{
    public interface IGameListener
    {
        
    }
    
    public interface IBattleChangeListener : IGameListener
    {
        void OnBattleChange(int battleIndex);
    }
    
    public interface IAgeChangeListener : IGameListener
    {
        void OnAgeChange(int age);
    }
    
    public interface ITimelineChangeListener : IGameListener
    {
        void OnTimelineChange(int timeline);   
    }
    
    public interface ILevelChangeListener : IGameListener
    {
        void OnLevelChange(int level);   
    }
}