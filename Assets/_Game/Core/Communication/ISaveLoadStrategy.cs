using _Game.Core.UserState;
using _Game.Core.UserState._State;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Communication
{
    public interface ISaveLoadStrategy
    {
        UniTask<bool> SaveUserState(UserAccountState state, string path);
        UniTask<UserAccountState> GetUserState(string path);
    }
}