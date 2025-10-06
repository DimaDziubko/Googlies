using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._StartBattleScreen.Scripts
{
    public interface IStartBattleScreenProvider
    {
        UniTask<Disposable<StartBattleScreen>> Load();
        void Dispose();
        StartBattleScreen GetScreen();
    }
}