using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Core.Navigation.Timeline
{
    public class DungeonModel : IDungeonModel
    {
        private const int LEVEL_PER_STAGE = 10;

        public event Action LevelChanged;
        public event Action<DungeonType, int, int, int> LevelUpWithInfo;

        private readonly IMyLogger _logger;
        private readonly Dungeon _dungeon;
        private readonly DungeonConfig _config;

        private int _currentLevel;

        public int CurrentLevel
        {
            get => _currentLevel;
            private set
            {
                _currentLevel = value;
                LevelChanged?.Invoke();
            }
        }

        public float Difficulty
        {
            get
            {
                if (Stage <= _config.StartDifficulty.Count)
                    return _config.StartDifficulty[Stage - 1];
                if (_config.IsExponentionDifficultyUsed)
                    return _config.ExpDifficulty.GetValue(Stage);
                return _config.Difficulty.GetValue(Stage);
            }
        }

        public int TimelineId => Stage - 1;
        public int Stage => (_currentLevel - 1) / LEVEL_PER_STAGE + 1;
        public int SubLevel => (_currentLevel - 1) % LEVEL_PER_STAGE + 1;

        public int MaxAvailableLevel => _dungeon.Level;
        public int MaxAvailableStage => _dungeon.Level / LEVEL_PER_STAGE + 1;
        public int MaxAvailableSubLevel => _dungeon.Level % LEVEL_PER_STAGE + 1;

        public float FoodProductionSpeed => _config.FoodProduction;
        public int InitialFoodAmount => _config.InitialFoodAmount;

        public IEnumerable<CurrencyData> Reward => new[]
            { new CurrencyData() { Type = CurrencyType.Gems, Amount = GetRewardAmount, Source = ItemSource.Dungeons} };

        public bool IsLocked => _config.IsLocked;
        public int Delay => _config.Delay;

        public DungeonType DungeonType => _dungeon.DungeonType;
        public int SequenceCooldown => _config.Cooldown;
        public string EnvironmentKey => _config.EnvironmentKey;
        public string AmbienceKey => _config.AmbienceKey;
        public int KeysCount => _dungeon.KeysCount;
        public int VideosCount => _dungeon.VideosCount;
        public string Name => _config.Name;
        public int MaxLevel => _dungeon.MaxLevel;
        public float GetRewardAmount => _config.RewardFunction.GetInt(CurrentLevel);
        public int RequiredTimeline => _config.RequiredTimeline;
        public Dungeon Dungeon => _dungeon;
        public Sprite Icon => _config.Icon;
        public int MaxVideosCount => _config.VideosCount;
        public int MaxKeysCount => _config.KeysCount;
        public Sprite AdsIcon => _config.AdsIcon;
        public Sprite KeyIcon => _config.KeyIcon;
        public Sprite RewardIcon => _config.RewardIcon;

        public DungeonModel(
            Dungeon dungeon,
            DungeonConfig config,
            IMyLogger logger)
        {
            _dungeon = dungeon;
            _logger = logger;
            _config = config;
            _currentLevel = dungeon.Level;
        }


        public void MoveToNextLevel()
        {
            if (CanMoveNext())
            {
                CurrentLevel++;
            }
        }

        public void MoveToPreviousLevel()
        {
            if (CanMovePrevious())
            {
                CurrentLevel--;
            }
        }

        public bool CanMoveNext() => CurrentLevel < _dungeon.Level;

        public bool CanMovePrevious() => CurrentLevel > 1;

        public short GetWarriorsCount(UnitType unitType)
        {
            _logger.Log($"GetWarriorsCount called with SubLevel = {SubLevel}, UnitType = {unitType}",
                DebugStatus.Warning);

            short rawLight = _config.LightWarriorCountFunction.GetShort(SubLevel);
            short rawMedium = _config.MediumWarriorCountFunction.GetShort(SubLevel);
            short rawHeavy = _config.HeavyWarriorCountFunction.GetShort(SubLevel);

            _logger.Log($"[Raw] Light = {rawLight}, Medium = {rawMedium}, Heavy = {rawHeavy}", DebugStatus.Warning);

            short finalLight = (short)Mathf.Max(0, rawLight);
            short finalMedium = (short)Mathf.Max(0, rawMedium);
            short finalHeavy = (short)Mathf.Max(0, rawHeavy);

            _logger.Log($"[Clamped] Light = {finalLight}, Medium = {finalMedium}, Heavy = {finalHeavy}",
                DebugStatus.Warning);

            switch (unitType)
            {
                case UnitType.Light:
                    _logger.Log($"Result for Light = {finalLight}", DebugStatus.Warning);
                    return finalLight;
                case UnitType.Medium:
                    _logger.Log($"Result for Medium = {finalMedium}", DebugStatus.Warning);
                    return finalMedium;
                case UnitType.Heavy:
                    _logger.Log($"Result for Heavy = {finalHeavy}", DebugStatus.Warning);
                    return finalHeavy;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        public void LevelUp()
        {
            _dungeon.LevelUp();
            LevelUpWithInfo?.Invoke(
                DungeonType,
                MaxAvailableLevel,
                MaxAvailableStage,
                MaxAvailableSubLevel);
        }

        public void SpendKey() => _dungeon.SpendKey();

        public void SpendVideo() => _dungeon.SpendVideo();
        public void AddKey(ItemSource keySource) => _dungeon.AddKey(keySource);
        public void AddKeys(int amount, ItemSource source) => _dungeon.AddKey(amount, source);
    }
}