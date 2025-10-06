using System;
using _Game.Core._Logger;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class BaseClearingOperation : ILoadingOperation
    {
        private readonly BaseFactoryMediator _mediator;
        private readonly IMyLogger _logger;

        public BaseClearingOperation(
            BaseFactoryMediator mediator,
            IMyLogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string Description => "Clearing bases...";
        public UniTask Load(Action<float> onProgress)
        {
            _mediator.ClearBases();
            _logger.Log("Bases clearing operation completed", DebugStatus.Info);
            return UniTask.CompletedTask;
        }
    }
}