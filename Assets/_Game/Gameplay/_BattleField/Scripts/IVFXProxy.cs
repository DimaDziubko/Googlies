using _Game.Core.Configs.Models._Skills;
using _Game.Core.UserState._State;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IVFXProxy
    {
        IVfxFactory VfxFactory { get;}
        void SpawnUnitVfx(Vector3 position);
        UniTask SpawnMuzzleFlash(MuzzleData muzzleData);
        UniTask SpawnProjectileExplosion(string key, Vector3 position);
        void SpawnDamageTextLeft(Vector3 position, string value);
        void SpawnDamageTextRight(Vector3 unitPosition, string toCompactFormat);
        UniTask SpawnMeteorExplosion(string key, Vector3 position);
        UniTask SpawnMeteor(Vector3 position, Vector3 destination, MeteorConfig config, string key, float damageToApply);
        void SpawnPush(Vector3 position);
        void SpawnRecover(Vector3 position, Sprite sprite, string value);
        void SpawnBattlePassLoot(Vector3 position);
    }
}