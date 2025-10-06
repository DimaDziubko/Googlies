using System;
using System.Collections.Generic;
using _Game.UI._TimelineInfoScreen.Scripts;

namespace _Game.UI._TimelineInfoPresenter
{
    public interface ITimelineInfoPresenter
    {
        event Action StateChanged;
        IEnumerable<TimelineInfoItem> Items { get; }
        int CurrentAge { get; }
        string GetDifficulty();
        string GetTimelineText();
    }
}