using _Game.UI.BattleResultPopup.Scripts;

namespace _Game.Core._GameListenerComposite
{
    public interface IEndGameListener : IGameListener
    {
        void OnEndBattle(GameResultType result, bool wasExit);
    }
}