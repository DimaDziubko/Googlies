using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public interface IUpgradesScreenProvider
    {
        UniTask<Disposable<UpgradesScreen>> Load();
        void Dispose();
    }
}