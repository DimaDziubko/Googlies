using System;

namespace Assets._Game.Core.UserState
{
    public interface IBattleSpeedStateReadonly
    {
        bool IsNormalSpeedActive { get; }
        int PermanentSpeedId { get; }
        float DurationLeft { get; }
        
        event Action<bool> IsNormalSpeedActiveChanged;
        event Action<int> PermanentSpeedChanged;
        event Action<float> DurationLeftChanged;
    }
}