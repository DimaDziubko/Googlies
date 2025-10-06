using System.IO;
using _Game.Core.UserState._State;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Communication
{
    public class LocalUserStateCommunicator : IUserStateCommunicator
    {
        private readonly ISaveLoadStrategy _strategy;
        private string SaveFolder => $"{Application.persistentDataPath}/game_saves";
        private string FileName => "userAccountState.json";
        private string Path => $"{SaveFolder}/{FileName}";

        public LocalUserStateCommunicator(ISaveLoadStrategy strategy)
        {
            _strategy = strategy;
            EnsureSaveFolderExists();
        }

        private void EnsureSaveFolderExists()
        {
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }
        }

        public async UniTask<bool> SaveUserState(UserAccountState state)
        {
            state.Version = Application.version;
            return await _strategy.SaveUserState(state, Path);
        }

        public async UniTask<UserAccountState> GetUserState()
        {
            var result =  await _strategy.GetUserState(Path);
            return result;
        }
    }
}