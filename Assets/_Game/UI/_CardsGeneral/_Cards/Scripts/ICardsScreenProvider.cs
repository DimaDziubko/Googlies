using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsScreenProvider
    {
        UniTask<Disposable<CardsScreen>> Load();
        void Dispose();
    }
}