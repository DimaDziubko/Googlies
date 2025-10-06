using System;
using Assets._Game.Core.UserState;

namespace _Game.Core.UserState._State
{
    public class BattleSpeedState : IBattleSpeedStateReadonly
    {
        public bool IsNormalSpeedActive;
        public int PermanentSpeedId;
        public float DurationLeft;
        
        public event Action<bool> IsNormalSpeedActiveChanged;
        public event Action<int> PermanentSpeedChanged;
        public event Action<float> DurationLeftChanged;

        bool IBattleSpeedStateReadonly.IsNormalSpeedActive => IsNormalSpeedActive;
        int IBattleSpeedStateReadonly.PermanentSpeedId => PermanentSpeedId;
        float IBattleSpeedStateReadonly.DurationLeft => DurationLeft;

        public void SetNormalSpeedActive(bool isActive)
        {
            IsNormalSpeedActive = isActive;
            IsNormalSpeedActiveChanged?.Invoke(isActive);
        }
        
        public void ChangePermanentSpeed(int id)
        {
            PermanentSpeedId = id;
            PermanentSpeedChanged?.Invoke(id);
        }

        public void ChangeDurationLeft(float timeLeft)
        {
            DurationLeft = timeLeft;
            DurationLeftChanged?.Invoke(timeLeft);
        }
    }
}