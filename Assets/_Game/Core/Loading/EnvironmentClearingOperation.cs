using System;
using _Game.Core._Logger;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class EnvironmentClearingOperation : ILoadingOperation
    {
        private readonly EnvironmentFactoryMediator _mediator;
        private readonly IMyLogger _logger;

        public EnvironmentClearingOperation(
            EnvironmentFactoryMediator mediator,
            IMyLogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public string Description => "Clearing environment...";
        public UniTask Load(Action<float> onProgress)
        {
            _mediator.ClearEnvironment();
            _logger.Log("Environment clearing operation completed", DebugStatus.Info);
            return UniTask.CompletedTask;
        }
    }
}