using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._BattleResultPopup.Scripts
{
    public interface IBattleResultPopupProvider
    {
        UniTask<Disposable<BattleResultPopup>> Load();
        void Dispose();
    }
}