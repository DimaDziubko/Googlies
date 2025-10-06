using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._BoostPopup
{
    public interface IBoostPopupProvider
    {
        UniTask<Disposable<BoostPopup>> Load();
        void Dispose();
    }
}