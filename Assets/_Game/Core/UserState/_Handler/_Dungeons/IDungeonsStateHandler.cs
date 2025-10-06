using _Game.Core.UserState._State;

namespace _Game.Core.UserState._Handler._Dungeons
{
    public interface IDungeonsStateHandler
    {
        bool HasDungeon(int id);
        void AddDungeon(Dungeon dungeon);
        void RemoveDungeon(int id);
    }
}