namespace Assets._Game.Core.Communication
{
    // public class BinaryFormatterSaveLoadStrategy : ISaveLoadStrategy
    // {
    //     public async UniTask<bool> SaveUserState(UserAccountState state, string path)
    //     {
    //         // using (var stream = File.Open(path, FileMode.Create))
    //         // {
    //         //     var formatter = new BinaryFormatter();
    //         //     formatter.Serialize(stream, state);
    //         // }
    //         // await UniTask.CompletedTask;
    //         return true;
    //     }
    //
    //     public async UniTask<UserAccountState> GetUserState(string path)
    //     {
    //         // if (!File.Exists(path)) return null;
    //         //
    //         // using (var stream = File.Open(path, FileMode.Open))
    //         // {
    //         //     var formatter = new BinaryFormatter();
    //         //     var state = (UserAccountState)formatter.Deserialize(stream);
    //         //     return state;
    //         // }
    //         
    //         return new UserAccountState();
    //     }
    // }
}