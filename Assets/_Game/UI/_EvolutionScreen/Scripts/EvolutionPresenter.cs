using _Game.Core._Logger;
using _Game.UI._EvolveScreen.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using System;

namespace _Game.UI._EvolutionScreen
{
    public class EvolutionPresenter : IDisposable
    {
        private readonly IEvolveScreenProvider _provider;
        private readonly IMyLogger _logger;

        private Disposable<EvolveScreen> _evolveScreen;

        public EvolutionPresenter(
            IEvolveScreenProvider provider,
            IMyLogger logger
            )
        {
            _provider = provider;
            _logger = logger;
        }

        public void Dispose()
        {
            _provider.Dispose();
            _evolveScreen = null;
        }

        public async UniTask ShowScreen()

        {
            _evolveScreen = await _provider.Load();
            _evolveScreen.Value.Show();
        }
    }
}
