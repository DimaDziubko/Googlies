using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Settings.Scripts
{
    public interface ISettingsPopupProvider 
    {
        UniTask<Disposable<SettingsPopup>> Load();
    }
}