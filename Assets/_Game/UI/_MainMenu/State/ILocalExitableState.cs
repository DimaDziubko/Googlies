using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.State
{
    public interface ILocalExitableState
    {
        UniTask InitializeAsync();
        void SetActive(bool isActive);
        void Enter();
        void Exit();
        void Cleanup();
    }
}