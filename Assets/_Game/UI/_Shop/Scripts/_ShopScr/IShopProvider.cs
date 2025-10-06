using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public interface IShopProvider 
    {
        UniTask<Disposable<Shop>> Load();
        void Dispose();
    }
}