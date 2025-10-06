using Cysharp.Threading.Tasks;

namespace _Game._BattleModes.Scripts
{
    public interface IResetable
    {
        UniTask Reset();
    }
}