using _Game.Gameplay.Vfx.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Factory
{
    public interface IVfxFactory
    {
        void Warmup();
        DamageText GetDamageText();
        void Reclaim<T>(T entity) where T : VfxEntity;
        T Get<T>() where T : VfxEntity;
        T Get<T>(Transform parent) where T : VfxEntity;
        UniTask<T> GetAsync<T>(string key) where T : KeyedVfxEntity;
        void Reclaim<T>(string key, T instance) where T : KeyedVfxEntity;
        void ReclaimDamageText(DamageText damageText);
    }
}