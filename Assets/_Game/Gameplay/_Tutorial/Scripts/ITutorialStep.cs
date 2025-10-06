using System;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialStep
    {
        event Action<ITutorialStep> Show;
        event Action<ITutorialStep> Complete;
        event Action<ITutorialStep> Cancel;

        TutorialStepData GetTutorialStepData();
    }
}