using System;
using _Game.Core.Loading;
using Cysharp.Threading.Tasks;

namespace _Game._BattleModes.Scripts
{
    public class ResetOperation : ILoadingOperation
    {
        private readonly IResetable _resetable;

        public ResetOperation(IResetable resetable)
        {
            _resetable = resetable;
        }

        public string Description => "Reset...";
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.1f);
            await _resetable.Reset();
            onProgress?.Invoke(1f);
        }
    }
}