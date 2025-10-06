using System;

namespace _Game.Core.Navigation.Timeline
{
    public interface ITimelineNavigator
    {
        event Action TimelineChanged;
        int CurrentTimelineNumber { get;}
        int CurrentTimelineId { get;}
        void MoveToNextTimeline();
        void MoveToPreviousTimeline();
    }
}