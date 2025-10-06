using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public interface ITimelineInfoScreenProvider
    {
        UniTask<Disposable<TimelineInfoScreen>> Load();
        void Dispose();
    }
}