using System.Threading;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral.Scripts
{
    public interface IGeneralCardsScreenProvider
    {
        UniTask<Disposable<GeneralCardsScreen>> Load();
        void Dispose();
    }
}