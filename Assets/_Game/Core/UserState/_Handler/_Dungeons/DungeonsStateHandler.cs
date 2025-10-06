using System.Linq;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;

namespace _Game.Core.UserState._Handler._Dungeons
{
    public class DungeonsStateHandler : IDungeonsStateHandler
    {
        private readonly IUserContainer _userContainer;

        private DungeonsSavegame Dungeons => _userContainer.State.DungeonsSavegame;
        
        public DungeonsStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public bool HasDungeon(int id) => 
            Dungeons.Dungeons.Any(x=>x.Id == id);
        
        public void AddDungeon(Dungeon dungeon)
        {
            if (!HasDungeon(dungeon.Id))
            {
                Dungeons.Dungeons.Add(dungeon);
            }
        }

        public void RemoveDungeon(int id)
        {
            var dungeonToRemove = Dungeons.Dungeons.FirstOrDefault(x => x.Id == id);
            if (dungeonToRemove != null)
            {
                Dungeons.Dungeons.Remove(dungeonToRemove);
            }
        }
    }
}