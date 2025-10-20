using System;
using UnityEngine;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineInfoItem
    {
        public event Action StateChanged;
        public bool IsLocked => _isLocked;
        public bool IsGained => _isGained;

        private bool _isLocked;
        private bool _isGained;

        public void SetLocked(bool isLocked)
        {
            _isLocked = isLocked;
            StateChanged?.Invoke();
        }

        public void SetGained(bool isGained)
        {
            _isGained = isGained;
            StateChanged?.Invoke();
        }

        public Sprite Icon { get; private set; }
        public string Name { get; private set; }
        public string DateRange { get; private set; }
        public string Description { get; private set; }


        public class TimelineInfoItemBuilder
        {
            private Sprite _icon;
            private string _name;
            private string _dateRange;
            private string _description;

            public TimelineInfoItemBuilder WithIcon(Sprite icon)
            {
                _icon = icon;
                return this;
            }

            public TimelineInfoItemBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public TimelineInfoItemBuilder WithDateRange(string dateRange)
            {
                _dateRange = dateRange;
                return this;
            }

            public TimelineInfoItemBuilder WithDescription(string description)
            {
                _description = description;
                return this;
            }

            public TimelineInfoItem Build()
            {
                var baseData = new TimelineInfoItem
                {
                    Icon = _icon,
                    Description = _description,
                    DateRange = _dateRange,
                    Name = _name,
                };
                return baseData;
            }
        }
    }
}