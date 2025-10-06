using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Core.Configs.Models._TimelineConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Models._WeaponConfig;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Repositories.Timeline
{
    public interface ILevelConfig
    {
        int Level { get; }
    }

    public class TimelineConfigRepository : ITimelineConfigRepository
    {
        private const int MAX_LEVEL_COUNT = 6;

        private readonly IUserContainer _userContainer;
        private readonly Dictionary<int, TimelineConfig> _cache = new();
        private readonly IMyLogger _logger;

        public TimelineConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public WeaponConfig ForWeapon(int weaponId) =>
            GetConfigById(_userContainer.GameConfig.WeaponConfigs, weaponId, "Weapon");

        public AgeConfig[] GetAgeConfigs(int timelineId) =>
            GetTimeline(timelineId).AgeIds
                .Select(id => _userContainer.GameConfig.AgeConfigs[id])
                .ToArray();

        public AgeConfig GetRelatedAge(int timelineId, int ageIdx)
        {
            var timeline = GetTimeline(timelineId);

            if (!IsValidIndex(timeline.AgeIds, ageIdx, "Age", timelineId))
            {
                GetConfigById(_userContainer.GameConfig.AgeConfigs, timeline.AgeIds[0], "Age");
                _logger.Log($"Timeline [{timelineId}] does not have any related ages.", DebugStatus.Warning);
            }

            return GetConfigById(_userContainer.GameConfig.AgeConfigs, timeline.AgeIds[ageIdx], "Age");
        }

        public BattleConfig GetRelatedBattle(int timelineId, int battleIdx)
        {
            var timeline = GetTimeline(timelineId);

            if (!IsValidIndex(timeline.AgeIds, battleIdx, "Battle", timelineId))
                _logger.LogError($"Timeline [{timelineId}] does not have any related battle.");

            return GetConfigById(_userContainer.GameConfig.BattleConfigs, timeline.BattleIds[battleIdx], "Battle");
        }

        public WarriorConfig GetAgeRelatedWarrior(int timelineId, int ageIdx, UnitType type)
        {
            var age = GetRelatedAge(timelineId, ageIdx);
            int warriorId = age.WarriorsId[(int)type];

            if (age == null || !IsValidIndex(age.WarriorsId, (int)type, "Warrior", timelineId))
                _logger.LogError($"Warriors does not have id {warriorId}.");

            return GetConfigById(_userContainer.GameConfig.WarriorConfigs, age.WarriorsId[(int)type], "Warrior");
        }

        public WarriorConfig GetBattleRelatedWarrior(int timelineId, int battleIdx, UnitType type)
        {
            var battle = GetRelatedBattle(timelineId, battleIdx);
            int warriorId = battle.WarriorsId[(int)type];

            if (battle == null || !IsValidIndex(battle.WarriorsId, (int)type, "Warrior", timelineId))
                _logger.LogError($"Warriors does not have id {warriorId}.");

            return GetConfigById(_userContainer.GameConfig.WarriorConfigs, battle.WarriorsId[(int)type], "Warrior");
        }

        public IEnumerable<WarriorConfig> GetRelatedAgeWarriors(int timelineId, int ageIdx)
        {
            var age = GetRelatedAge(timelineId, ageIdx);
            return age != null ? GetWarriorsByIds(age.WarriorsId) : Enumerable.Empty<WarriorConfig>();
        }

        public IEnumerable<WarriorConfig> GetRelatedBattleWarriors(int timelineId, int battleIdx)
        {
            var battle = GetRelatedBattle(timelineId, battleIdx);
            return battle != null ? GetWarriorsByIds(battle.WarriorsId) : Enumerable.Empty<WarriorConfig>();
        }

        public List<BattleConfig> GetBattleConfigs(int timelineId) =>
            GetTimeline(timelineId).BattleIds
                .Select(id => _userContainer.GameConfig.BattleConfigs[id])
                .ToList();

        public int LastBattleIndex() => MAX_LEVEL_COUNT - 1;
        public int LastAgeIdx() => MAX_LEVEL_COUNT - 1;
        public int AgesCount(int timelineId) => GetTimeline(timelineId).AgeIds.Count;
        public int LastBattle(int timelineId) => GetTimeline(timelineId).BattleIds.Count;

        public TimelineConfig GetTimeline(int timelineId)
        {
            if (_cache.TryGetValue(timelineId, out var cachedTimeline))
            {
                return cachedTimeline;
            }

            TimelineConfig newTimeline = GenerateTimeline(timelineId);
            _cache[timelineId] = newTimeline;
            return newTimeline;
        }

        private TimelineConfig GenerateTimeline(int timelineId)
        {
            var timelineConfig = new TimelineConfig
            {
                AgeIds = new List<int>(),
                BattleIds = new List<int>()
            };

            for (int level = 1; level <= MAX_LEVEL_COUNT; level++)
            {
                List<int> ageLevelGroup = _userContainer
                    .GameConfig
                    .AgeConfigs
                    .Values
                    .Where(x => x.Level == level)
                    .OrderBy(x => x.Order)
                    .Select(x => x.Id).ToList();


                string group = "";
                
                foreach (int ageId in ageLevelGroup)
                {
                    group += $"{ageId} ";
                    
                }
                
                _logger.Log($"AGE LEVEL GROUP {group}", DebugStatus.Info);
                
                var permutations = GetAllPermutations(ageLevelGroup);
                var currentPermutation = GenerateCollectionsWithShifts(permutations, excludeElements: 1, level - 1);
                var mergedList = MergePermutationsWithoutRepeats(currentPermutation);
                int index = (timelineId < mergedList.Count) ? timelineId : timelineId % mergedList.Count;
                int configId = mergedList[index];


                timelineConfig.AgeIds.Add(configId);
                timelineConfig.BattleIds.Add(configId);

            }

            string timeline = "";
                
            foreach (int ageId in timelineConfig.AgeIds)
            {
                timeline += $"{ageId} ";
            }
                
            _logger.Log($"TIMELINE {timelineConfig.Id} {timeline}", DebugStatus.Info);
            
            return timelineConfig;
        }

        private List<int> MergePermutationsWithoutRepeats(List<List<int>> permutations)
        {
            var mergedList = new List<int>();

            foreach (var perm in permutations)
            {
                foreach (var num in perm)
                {
                    if (mergedList.Count > 0 && mergedList.Last() == num)
                    {
                        continue;
                    }

                    mergedList.Add(num);
                }
            }

            if (mergedList.First() == mergedList.Last())
            {
                mergedList.RemoveAt(mergedList.Count - 1);
            }

            return mergedList;
        }

        private List<List<int>> GenerateCollectionsWithShifts(List<List<int>> permutations, int excludeElements,
            int shiftStep)
        {
            var newPermutations = new List<List<int>>();

            foreach (var perm in permutations)
            {
                newPermutations.Add(perm);
            }

            for (int i = 0; i < shiftStep; i++)
            {
                List<int> last = newPermutations[^1];
                newPermutations.RemoveAt(newPermutations.Count - 1);
                newPermutations.Insert(excludeElements, last);
            }

            return newPermutations;
        }


        private List<List<int>> GetAllPermutations(List<int> list)
        {
            var permutations = new List<List<int>>();
            var result = GetPermutations(list, list.Count);
            foreach (var perm in result)
            {
                permutations.Add(perm.ToList());
            }

            return permutations;
        }

        private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t, e) => t.Concat(new T[] { e }));
        }

        private bool IsValidIndex<T>(List<T> list, int index, string entityName, int timelineId)
        {
            if (index < 0 || index >= list.Count)
            {
                _logger.LogError($"Timeline [{timelineId}] does not have {entityName} at index {index}.");
                return false;
            }

            return true;
        }

        private List<TConfig> GetConfigsByLevel<TConfig>(IEnumerable<TConfig> configs, int level)
            where TConfig : ILevelConfig
        {
            return configs.Where(config => config.Level == level).ToList();
        }

        private TConfig GetConfigById<TConfig>(Dictionary<int, TConfig> dictionary, int id, string entityName)
        {
            if (!dictionary.TryGetValue(id, out var config))
            {
                _logger.LogError($"{entityName} does not have id {id}.");
                return default;
            }

            return config;
        }

        private IEnumerable<WarriorConfig> GetWarriorsByIds(IEnumerable<int> warriorIds)
        {
            foreach (var warriorId in warriorIds)
            {
                var warrior = GetConfigById(_userContainer.GameConfig.WarriorConfigs, warriorId, "Warrior");
                if (warrior != null) yield return warrior;
            }
        }

        public IEnumerable<WarriorConfig> GetAllBattleWarriors(int timelineId)
        {
            var timeline = GetTimeline(timelineId);

            foreach (var battleId in timeline.BattleIds)
            {
                if (_userContainer.GameConfig.BattleConfigs.TryGetValue(battleId, out var battleConfig))
                {
                    foreach (var warriorId in battleConfig.WarriorsId)
                    {
                        if (_userContainer.GameConfig.WarriorConfigs.TryGetValue(warriorId, out var warriorConfig))
                        {
                            yield return warriorConfig;
                        }
                        else
                        {
                            _logger.LogError($"WarriorConfig not found for ID: {warriorId} in Battle ID: {battleId}");
                        }
                    }
                }
                else
                {
                    _logger.LogError($"BattleConfig not found for Battle ID: {battleId} in Timeline ID: {timelineId}");
                }
            }
        }

        public IEnumerable<WarriorConfig> GetAllAgeWarriors(int timelineId)
        {
            var timeline = GetTimeline(timelineId);

            foreach (var ageId in timeline.AgeIds)
            {
                if (_userContainer.GameConfig.BattleConfigs.TryGetValue(ageId, out var ageConfig))
                {
                    foreach (var warriorId in ageConfig.WarriorsId)
                    {
                        if (_userContainer.GameConfig.WarriorConfigs.TryGetValue(warriorId, out var warriorConfig))
                        {
                            yield return warriorConfig;
                        }
                        else
                        {
                            _logger.LogError($"WarriorConfig not found for ID: {warriorId} in Age ID: {ageId}");
                        }
                    }
                }
                else
                {
                    _logger.LogError($"BattleConfig not found for Battle ID: {ageId} in Timeline ID: {timelineId}");
                }
            }
        }
    }
}