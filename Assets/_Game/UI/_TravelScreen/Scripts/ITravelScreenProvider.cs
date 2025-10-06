using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._TravelScreen.Scripts
{
    public interface ITravelScreenProvider
    {
        UniTask<Disposable<TravelScreen>> Load();
        void Dispose();
    }
}