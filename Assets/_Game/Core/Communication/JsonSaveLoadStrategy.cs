using System.IO;
using System.Threading;
using _Game.Core.UserState._State;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Game.Core.Communication
{
    public class JsonSaveLoadStrategy : ISaveLoadStrategy
    {
        private readonly StateMigrationManager _migrationManager = new();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public async UniTask<bool> SaveUserState(UserAccountState state, string path)
        {
            await _semaphore.WaitAsync();

            try
            {
                state.Version = Application.version;
                string json = JsonConvert.SerializeObject(state);
                await File.WriteAllTextAsync(path, json);
            }
            finally
            {
                _semaphore.Release();
            }

            return true;
        }

        public async UniTask<UserAccountState> GetUserState(string path)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (!File.Exists(path)) return null;

                string json = await File.ReadAllTextAsync(path);

                var state = JsonConvert.DeserializeObject<UserAccountState>(json, new UserAccountStateConverter());

                if (state != null && state.Version != Application.version)
                {
                    _migrationManager.Migrate(ref state);
                }

                return state;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}