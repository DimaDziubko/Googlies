namespace _Game.UI.Common.Scripts
{
    public interface IGameScreen
    {
        bool IsReviewed { get; }
        bool NeedAttention { get; }
    }

    public interface IUpgradeUnitsScreen : IGameScreen
    {
        
    }
    
    public interface IDungeonScreen : IGameScreenWithInfo
    {
        
    }
    
    public interface IMenuScreen : IGameScreen
    {
        
    }

    public interface IGeneralCardsScreen : IGameScreen
    {
        
    }

    public interface IGameScreenWithInfo : IGameScreen
    {
        string Info { get; }
    }

    public interface IStartBattleScreen : IGameScreenWithInfo
    {
        
    }

    public interface IShopScreen : IGameScreenWithInfo
    {
        
    }

    public interface IEvolveScreen : IGameScreenWithInfo
    {
        
    }

    public interface ITravelScreen : IGameScreenWithInfo
    {
        
    }

    public interface IUpgradesScreen : IGameScreenWithInfo
    {
        
    }

    public interface ICardsScreen : IGameScreenWithInfo
    {
        
    }
    
    public interface ISkillsScreen : IGameScreenWithInfo
    {
        
    }
}