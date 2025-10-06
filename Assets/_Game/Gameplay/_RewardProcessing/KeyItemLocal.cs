using _Game.Core.UserState._State;

namespace _Game.Gameplay._RewardProcessing
{
    public class KeyItemLocal : ItemLocal
    {
        public override int Id { get; set; }
        public override string IconKey { get; set; }
        public DungeonType DungeonType;
    }
}