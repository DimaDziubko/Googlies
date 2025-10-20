using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._EvolveScreen.Scripts;
using _Game.UI._TravelScreen.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using System;

namespace _Game.UI._EvolutionScreen
{
    public class EvolutionPresenter : IDisposable
    {
        private readonly IEvolveScreenProvider _provider;
        private readonly ITravelScreenProvider _travelProvider;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IConfigRepository _config;

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
            _evolveScreen.Value.CloseClicked += Dispose;
        }
    }
}
