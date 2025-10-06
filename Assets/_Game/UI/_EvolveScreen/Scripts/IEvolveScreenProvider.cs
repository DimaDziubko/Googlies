using System.Threading;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._EvolveScreen.Scripts
{
    public interface IEvolveScreenProvider
    {
        UniTask<Disposable<EvolveScreen>> Load();
        void Dispose();
    }
}