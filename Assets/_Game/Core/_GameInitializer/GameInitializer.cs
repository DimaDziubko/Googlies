using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace _Game.Core._GameInitializer
{
    public class GameInitializer : IGameInitializer
    {
        private readonly List<Func<UniTask>> _initializationTasks = new();

        public event Action OnPreInitialization;
        public event Action OnMainInitialization;
        public event Action OnPostInitialization;

        public async UniTask InitAsync()
        {
            var tasks = _initializationTasks.Select(init => init()).ToList();
            await UniTask.WhenAll(tasks);
        }

        public void Init()
        {
            OnPreInitialization?.Invoke();
            OnMainInitialization?.Invoke();
            OnPostInitialization?.Invoke();
        }

        public void RegisterAsyncInitialization(Func<UniTask> initMethod) =>
            _initializationTasks.Add(initMethod);
    }
}