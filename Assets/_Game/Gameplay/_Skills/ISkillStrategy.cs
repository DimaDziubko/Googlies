using System;

namespace _Game.Gameplay._Skills
{
    public interface ISkillStrategy
    {
        event Action Activated;
        event Action Completed;
        bool Execute();
        void Interrupt();
    }
}