using System;
using System.Collections.Generic;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game.Core.LoadingScreen
{
    public interface ILoadingScreenProvider
    {
        event Action LoadingCompleted;
        UniTask LoadAndDestroy(ILoadingOperation loadingOperation, LoadingScreenType type);
        UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations, LoadingScreenType type);
    }
}