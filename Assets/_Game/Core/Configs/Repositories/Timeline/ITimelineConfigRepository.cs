using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Core.Configs.Models._TimelineConfig;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Models._WeaponConfig;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Repositories.Timeline
{
    public interface ITimelineConfigRepository
    {
        IEnumerable<WarriorConfig> GetRelatedAgeWarriors(int timelineId, int ageIdx);
        IEnumerable<WarriorConfig> GetRelatedBattleWarriors(int timelineId, int battleIdx);
        WeaponConfig ForWeapon(int weaponId);
        AgeConfig[] GetAgeConfigs(int timelineId);
        AgeConfig GetRelatedAge(int timelineId, int ageIdx);
        BattleConfig GetRelatedBattle(int timelineId, int battleIdx);
        WarriorConfig GetAgeRelatedWarrior(int timelineId, int ageIdx, UnitType type);
        WarriorConfig GetBattleRelatedWarrior(int timelineId, int battleIdx, UnitType type);
        List<BattleConfig> GetBattleConfigs(int timelineId);
        int LastBattleIndex();
        int LastAgeIdx();
        int AgesCount(int timelineId);
        int LastBattle(int timelineId);
        IEnumerable<WarriorConfig> GetAllBattleWarriors(int timelineId);
        IEnumerable<WarriorConfig> GetAllAgeWarriors(int timelineId);
        TimelineConfig GetTimeline(int timelineId);
    }
}