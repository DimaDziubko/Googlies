using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;

namespace _Game.Core.UserState._State
{
    public interface IDungeonsStateReadonly
    {
        public IEnumerable<Dungeon> Dungeons { get; }
        public bool Contains(int id, out Dungeon dungeon);
    }

    public class DungeonsSavegame : IDungeonsStateReadonly
    {
        IEnumerable<Dungeon> IDungeonsStateReadonly.Dungeons => Dungeons;

        public List<Dungeon> Dungeons;

        bool IDungeonsStateReadonly.Contains(int id, out Dungeon dungeon)
        {
            dungeon = Dungeons.FirstOrDefault(x => x.Id == id);
            return dungeon != null;
        }

        public void Add(Dungeon dungeon)
        {
            if (!Dungeons.Contains(dungeon))
            {
                Dungeons.Add(dungeon);
            }
        }

        public void Remove(Dungeon dungeon)
        {
            if (Dungeons.Contains(dungeon))
            {
                Dungeons.Remove(dungeon);
            }
        }
    }

    public class Dungeon
    {
        public event Action KeysCountChanged;
        public event Action<int, DungeonType, ItemSource> KeysAdded;
        public event Action VideosCountChanged;
        public event Action OnKeysAdded;
        
        public int Id;
        public DungeonType DungeonType;
        public int Level;
        public int MaxLevel;
        public int KeysCount;
        public int VideosCount;
        public DateTime LastDungeonDay;
        

        public void SpendKey()
        {
            KeysCount--;
            LastDungeonDay = DateTime.UtcNow;
            KeysCountChanged?.Invoke();
        }

        public void AddKey(ItemSource source)
        {
            KeysCount++;
            KeysCountChanged?.Invoke();
            KeysAdded?.Invoke(1, DungeonType, source);
        }
        
        public void AddKey(int count, ItemSource source)
        {
            KeysCount += count;
            KeysCountChanged?.Invoke();
            KeysAdded?.Invoke(count, DungeonType, source);
            OnKeysAdded?.Invoke();
        }

        public void ChangeKeyCount(int amount)
        {
            KeysCount = amount;
            KeysCountChanged?.Invoke();
        }
        
        public void SpendVideo()
        {
            VideosCount--;
            VideosCountChanged?.Invoke();
        }
        
        public void AddVideo()
        {
            VideosCount++;
            VideosCountChanged?.Invoke();
        }
        
        public void ChangeVideoCount(int amount)
        {
            VideosCount = amount;
            VideosCountChanged?.Invoke();
        }

        public void LevelUp() => Level++;
    }
}