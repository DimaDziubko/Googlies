using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.WarriorsFund.Scripts
{
    public interface IWarriorsFundClaimPopupProvider
    {
        UniTask<Disposable<WarriorsFundClaimPopup>> Load();
    }
}