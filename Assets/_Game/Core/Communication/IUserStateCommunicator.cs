using _Game.Core.UserState._State;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Communication
{
    public interface IUserStateCommunicator
    {
        UniTask<bool> SaveUserState(UserAccountState state);
        UniTask<UserAccountState> GetUserState();
    }
}