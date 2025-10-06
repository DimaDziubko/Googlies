using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface ITutorialStateReadonly
    {
        List<int> CompletedSteps { get; }
        event Action<int> StepsCompletedChanged;
    }

    public class TutorialState : ITutorialStateReadonly 
    {
        public List<int> CompletedSteps;
        public event Action<int> StepsCompletedChanged;
        List<int> ITutorialStateReadonly.CompletedSteps => CompletedSteps;
        
        public void AddCompletedStep(int step)
        {
            if(CompletedSteps.Contains(step)) return;
            CompletedSteps.Add(step);
            StepsCompletedChanged?.Invoke(step);
        }
    }
}