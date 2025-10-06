using System;
using Cysharp.Threading.Tasks;

namespace _Game.Core._GameInitializer
{
    public interface IGameInitializer
    {
        UniTask InitAsync();
        void Init();
        public event Action OnPreInitialization;
        public event Action OnMainInitialization;
        public event Action OnPostInitialization;

        void RegisterAsyncInitialization(Func<UniTask> initMethod);
    }
}