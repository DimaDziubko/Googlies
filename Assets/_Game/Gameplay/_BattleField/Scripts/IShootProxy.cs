using _Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IShootProxy
    {
        UniTask Shoot(ShootData shootData);
    }
}