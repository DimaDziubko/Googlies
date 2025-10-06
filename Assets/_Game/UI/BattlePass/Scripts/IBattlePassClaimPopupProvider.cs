using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.BattlePass.Scripts
{
    public interface IBattlePassClaimPopupProvider
    {
        UniTask<Disposable<BattlePassClaimPopup>> Load();
    }
}