using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._Skills;
using _Game.UI.Common.Scripts;

namespace _Game.UI._Skills.Scripts
{
    public interface ISkillService
    {
        event Action<List<SkillModel>> SkillsAdded;
        SkillGenerator Generator { get; }
        bool IsAnyUpgradeAvailable { get; }
        string SkillProgressInfo { get; }
        IReadOnlyDictionary<SkillType, SkillModel> Skills { get; }
        SkillSlotContainer SkillSlotContainer { get; }
        float GetX1SkillPrice();
        float GetX10SkillPrice();
        void SkillBundleBought(IProduct product);
        SkillModel GetSkillModel(SkillType type);
    }
}