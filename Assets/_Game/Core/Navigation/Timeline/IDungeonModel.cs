using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public interface IDungeonModel
    {
        event Action LevelChanged;
        event Action<DungeonType, int, int, int> LevelUpWithInfo;
        int CurrentLevel { get; }
        float Difficulty { get; }
        int TimelineId { get; }
        int Stage { get; }
        DungeonType DungeonType { get; }
        int SequenceCooldown { get; }
        string EnvironmentKey { get; }
        string AmbienceKey { get; }
        int KeysCount { get; }
        int VideosCount { get; }
        string Name { get; }
        int MaxLevel { get; }
        float GetRewardAmount { get; }
        int RequiredTimeline { get; }
        Dungeon Dungeon { get; }
        Sprite Icon { get; }
        int MaxVideosCount { get; }
        int MaxKeysCount { get; }
        Sprite AdsIcon { get; }
        Sprite KeyIcon { get; }
        Sprite RewardIcon { get; }
        int SubLevel { get; }
        float FoodProductionSpeed { get; }
        int InitialFoodAmount { get; }
        IEnumerable<CurrencyData> Reward { get; }
        bool IsLocked { get; }
        int Delay { get; }
        int MaxAvailableLevel { get; }
        int MaxAvailableStage { get; }
        int MaxAvailableSubLevel { get; }
        void MoveToNextLevel();
        void MoveToPreviousLevel();
        bool CanMoveNext();
        bool CanMovePrevious();
        short GetWarriorsCount(UnitType unitType);
        void LevelUp();
        void SpendKey();
        void SpendVideo();
        void AddKey(ItemSource keySource);
        void AddKeys(int amount, ItemSource source);
    }
}