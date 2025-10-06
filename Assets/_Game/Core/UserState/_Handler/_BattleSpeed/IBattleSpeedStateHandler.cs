namespace _Game.Core.UserState._Handler._BattleSpeed
{
    public interface IBattleSpeedStateHandler
    {
        void ChangeNormalSpeed(bool isNormal);
        void ChangeBattleSpeedTimerDurationLeft(float timerTimeLeft);
        void ChangePermanentSpeedId(int newId);
    }
}