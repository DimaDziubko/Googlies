using System;

namespace _Game.Core.UserState
{
    public class RetentionState : IRetentionStateReadonly
    {
        public bool FirstDayRetentionEventSent;
        public bool SecondDayRetentionEventSent;
        public DateTime FirstOpenTime;
        
        public event Action FirstDayRetentionEventSentChanged;
        public event Action SecondDayRetentionEventSentChanged;

        DateTime IRetentionStateReadonly.FirstOpenTime => FirstOpenTime;
        bool IRetentionStateReadonly.FirstDayRetentionEventSent => FirstDayRetentionEventSent;
        bool IRetentionStateReadonly.SecondDayRetentionEventSent => SecondDayRetentionEventSent;

        public void ChangeFirstDayRetentionEventSent(bool isSent)
        {
            FirstDayRetentionEventSent = isSent;
            FirstDayRetentionEventSentChanged?.Invoke();
        }
        
        public void ChangeSecondDayRetentionEventSent(bool isSent)
        {
            SecondDayRetentionEventSent = isSent;
            SecondDayRetentionEventSentChanged?.Invoke();
        }
        
    }

    public interface IRetentionStateReadonly
    {
        DateTime FirstOpenTime { get; }
        bool FirstDayRetentionEventSent { get; }
        bool SecondDayRetentionEventSent { get; }
        
        public event Action FirstDayRetentionEventSentChanged;
        public event Action SecondDayRetentionEventSentChanged;
    }
}