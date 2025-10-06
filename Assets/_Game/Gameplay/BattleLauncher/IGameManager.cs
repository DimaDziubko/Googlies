using _Game.Core._GameListenerComposite;
using _Game.UI.BattleResultPopup.Scripts;

namespace _Game.Gameplay.BattleLauncher
{
    public interface IGameManager
    {
        BattleState State { get; }
        bool IsPaused { get;}
        void StartBattle();
        void Register(IGameListener listener);
        void Unregister(IGameListener listener);
        void SetPaused(bool isPaused);
        void StopBattle();
        void EndBattle(GameResultType result, bool wasExit = false);
        void ChangeBattle(int battleIdx);
        void ChangeAge(int ageIdx);
        void ChangeTimeline(int timelineId);
        void ChangeLevel(int level);
    }
}