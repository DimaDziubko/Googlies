using System;
using _Game.Core.Loading;
using _Game.Core.Navigation.Battle;
using Cysharp.Threading.Tasks;

namespace _Game._BattleModes.Scripts
{
    public class BattleTransitionOperation : ILoadingOperation
    {
        private readonly IBattleNavigator _navigator;
        public string Description => "Battle transition";

        public BattleTransitionOperation(IBattleNavigator navigator) => 
            _navigator = navigator;

        public UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.2f);
            _navigator.OpenNextBattle();
            onProgress?.Invoke(1);
            return UniTask.CompletedTask;
        }
    }
}