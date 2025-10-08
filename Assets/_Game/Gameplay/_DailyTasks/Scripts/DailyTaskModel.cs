using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._Functions;
using _Game.Core.UserState._State;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskModel
    {
        public event Action DescriptionChanged;
        public event Action TargetChanged;

        public IDailyTask State { get; private set; }
        public DailyTaskType Type { get; private set; }
        public string Description { get; private set; }
        public int Target { get; private set; }
        public List<LinearFunction> TargetFunctions { get; private set; }
        public IReadOnlyList<CurrencyData> Reward { get; private set; }
        public string DailyInfo { get; private set; }
        public string AdditionalDescription { get; private set; }

        public string Progress =>
            $"{Mathf.Min(State.ProgressOnTask, Target).ToCompactFormat(999)}/{Target.ToCompactFormat(999)}";
        public float ProgressValue =>
            Target > 0
                ? Mathf.Clamp01((float)State.ProgressOnTask / Target)
                : 0f;
        public string CustomDescription =>
            $"{Description} {AdditionalDescription}";
        public bool IsReady => State.ProgressOnTask >= Target - float.Epsilon;

        public void SetTarget(int target)
        {
            Target = target;
            TargetChanged?.Invoke();
        }

        public void SetAdditionalDescription(string additionalDescription)
        {
            AdditionalDescription = additionalDescription;
            DescriptionChanged?.Invoke();
        }

        public class DailyTaskModelBuilder
        {
            private IDailyTask _state;
            private DailyTaskType _type;
            private string _description;
            private int _target;
            private List<LinearFunction> _targetFunctions;
            private IReadOnlyList<CurrencyData> _reward;
            private string _dailyInfo;

            public DailyTaskModelBuilder WithState(IDailyTask state)
            {
                _state = state;
                return this;
            }

            public DailyTaskModelBuilder WithType(DailyTaskType type)
            {
                _type = type;
                return this;
            }

            public DailyTaskModelBuilder WithDescription(string description)
            {
                _description = description;
                return this;
            }

            public DailyTaskModelBuilder WithTarget(int target)
            {
                _target = target;
                return this;
            }

            public DailyTaskModelBuilder WithReward(IReadOnlyList<CurrencyData> reward)
            {
                _reward = reward;
                return this;
            }

            public DailyTaskModelBuilder WithTargetFunctions(List<LinearFunction> targetFunctions)
            {
                _targetFunctions = targetFunctions;
                return this;
            }

            public DailyTaskModelBuilder WithDailyInfo(string dailyInfo)
            {
                _dailyInfo = dailyInfo;
                return this;
            }


            public DailyTaskModel Build()
            {
                var unitData = new DailyTaskModel
                {
                    TargetFunctions = _targetFunctions,
                    State = _state,
                    Type = _type,
                    Description = _description,
                    Target = _target,
                    Reward = _reward,
                    DailyInfo = _dailyInfo
                };
                return unitData;
            }
        }
    }
}