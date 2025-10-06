using System.Threading;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._UpgradesAndEvolution.Scripts
{
    public interface IUpgradeAndEvolutionScreenProvider
    {
        UniTask<Disposable<UpgradeAndEvolutionScreen>> Load();
        void Dispose();
    }
}