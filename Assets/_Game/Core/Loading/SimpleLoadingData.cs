using System.Collections.Generic;
using _Game.Core.LoadingScreen;

namespace _Game.Core.Loading
{
    public interface ILoadingData
    {
        LoadingScreenType Type { get; }
        Queue<ILoadingOperation> Operations { get; }
    }

    public class SimpleLoadingData : ILoadingData
    {
        public LoadingScreenType Type { get; }
        public Queue<ILoadingOperation> Operations { get; }

        public SimpleLoadingData(LoadingScreenType type, Queue<ILoadingOperation> operations)
        {
            Type = type;
            Operations = operations;
        }
    }
}