using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public interface IMiniShopProvider
    {
        bool IsUnlocked { get; }
        UniTask<Disposable<MiniShop>> Load();
        void Dispose();
    }
}