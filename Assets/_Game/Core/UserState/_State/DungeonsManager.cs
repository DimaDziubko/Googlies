using System;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Core.Services.UserContainer;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;

namespace _Game.Core.UserState._State
{
    public class DungeonsManager : 
        IDisposable,
        IGameScreenListener<IMenuScreen>
    {
       private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private IDungeonsStateReadonly DungeonsState => _userContainer.State.DungeonsSavegame;

        public DungeonsManager(
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            UpdateDungeonsState();
        }

        private void UpdateDungeonsState()
        {
            foreach (var dungeonConfig in _userContainer.GameConfig.Dungeons.Values)
            {
                if (DungeonsState.Contains(dungeonConfig.Id, out var dungeon))
                {
                    CheckRecovering(dungeon, dungeonConfig);
                }
                else
                {
                    CreateNewDungeon(dungeonConfig);
                }
            }

            foreach (var dungeon in DungeonsState.Dungeons)
            {
                if (!_userContainer.GameConfig.Dungeons.ContainsKey(dungeon.Id))
                {
                    _userContainer.State.DungeonsSavegame.Remove(dungeon);
                }
            }
        }

        private void CheckRecovering(Dungeon dungeon, DungeonConfig dungeonConfig)
        {
            DateTime now = DateTime.UtcNow;
            
            if (now.Date > dungeon.LastDungeonDay)
            {
                if(dungeon.KeysCount < dungeonConfig.KeysCount) 
                    dungeon.ChangeKeyCount(dungeonConfig.KeysCount); 
                if(dungeon.VideosCount < dungeonConfig.VideosCount)
                    dungeon.ChangeVideoCount(dungeonConfig.VideosCount); 
                return;
            }
            
            TimeSpan recoveryTime = TimeSpan.FromMinutes(dungeonConfig.RecoveryTimeMinutes);
            
            if (now - dungeon.LastDungeonDay > recoveryTime)
            {
                if(dungeon.KeysCount < dungeonConfig.KeysCount) 
                    dungeon.ChangeKeyCount(dungeonConfig.KeysCount); 
                if(dungeon.VideosCount < dungeonConfig.VideosCount)
                    dungeon.ChangeVideoCount(dungeonConfig.VideosCount); 
            }
        }

        private void CreateNewDungeon(DungeonConfig dungeonConfig)
        {
            var dungeon = new Dungeon
            {
                Id = dungeonConfig.Id,
                DungeonType = dungeonConfig.Type,
                KeysCount = dungeonConfig.KeysCount,
                VideosCount = dungeonConfig.VideosCount,
                LastDungeonDay = DateTime.UtcNow,
                Level = 1,
                MaxLevel = 1
            };

            _userContainer.State.DungeonsSavegame.Add(dungeon);
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen) => UpdateDungeonsState();
        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}