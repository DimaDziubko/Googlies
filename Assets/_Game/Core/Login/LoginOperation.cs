using System;
using _Game.Core._Logger;
using _Game.Core.Communication;
using _Game.Core.Loading;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Login
{
    public class LoginOperation : ILoadingOperation
    {
        public string Description => "Login...";

        private Action<float> _onProgress;

        private readonly IUserContainer _userContainer;
        private readonly IUserStateCommunicator _communicator;
        private readonly IMyLogger _logger;

        public LoginOperation(
            IUserContainer userContainer,
            IUserStateCommunicator communicator,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _communicator = communicator;
            _logger = logger;
        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            _onProgress = onProgress;
            _onProgress?.Invoke(0.3f);

            _userContainer.State = await GetAccountState();

            _onProgress?.Invoke(1f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }

        private async UniTask<UserAccountState> GetAccountState()
        {
            UserAccountState result = await _communicator.GetUserState();

            _onProgress?.Invoke(0.6f);

            if (result == null || result.IsValid() == false)
            {
                result = UserAccountState.GetInitial();

                await _communicator.SaveUserState(result);
            }

            return result;
        }
    }

}