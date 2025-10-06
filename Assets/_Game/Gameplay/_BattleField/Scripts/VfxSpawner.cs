using System;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.UserState._State;
using _Game.Gameplay.Common;
using _Game.Gameplay.Vfx.Factory;
using _Game.Gameplay.Vfx.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class VfxSpawner : IVFXProxy
    {
        private readonly IVfxFactory _vfxFactory;
        private readonly ISettingsSaveGameReadonly _settings;
        private readonly IMyLogger _logger;
        
        [ShowInInspector, ReadOnly]
        private readonly GameBehaviourCollection _vfxEntities = new();

        public IVfxFactory VfxFactory => _vfxFactory;
        
        public VfxSpawner(
            IVfxFactory vfxFactory,
            ISettingsSaveGameReadonly settings,
            IMyLogger logger)
        {
            _logger = logger;
            _vfxFactory = vfxFactory;
            _settings = settings;
        }

        public void OnStartBattle() => 
            _vfxFactory.Warmup();

        public void GameUpdate(float deltaTime) => 
            _vfxEntities.GameUpdate(deltaTime);

        public void LateGameUpdate(float deltaTime) => 
            _vfxEntities.LateGameUpdate(deltaTime);

        async UniTask IVFXProxy.SpawnMuzzleFlash(MuzzleData data)
        {
            MuzzleFlash muzzle = await _vfxFactory.GetAsync<MuzzleFlash>(data.MuzzleKey);
            MuzzleFlash muzzleObject = muzzle.OrNull();
            if(muzzleObject.OrNull() == null) return;
            muzzleObject.Initialize(data.Position, data.Direction);
            _vfxEntities.Add(muzzleObject);
        }

        async UniTask IVFXProxy.SpawnProjectileExplosion(string key, Vector3 position)
        {
            ProjectileExplosion explosion = await _vfxFactory.GetAsync<ProjectileExplosion>(key);
            ProjectileExplosion explosionObject = explosion.OrNull();
            if(explosionObject.OrNull() == null) return;
            explosionObject.Initialize(position);
            _vfxEntities.Add(explosionObject);
        }

        private void SpawnDamageText(Vector3 position, string value, Action<DamageText, string, Vector3> playAction)
        {
            if (!_settings.IsDamageTextOn) return;

            var damageText = _vfxFactory.GetDamageText().OrNull();
            if (damageText.OrNull() == null) return;

            damageText.Initialize();
            playAction(damageText, value, position);
            _vfxEntities.Add(damageText);
        }

        void IVFXProxy.SpawnDamageTextLeft(Vector3 position, string value)
        {
            SpawnDamageText(position, value, (dt, v, pos) => dt.PlayDamageTextLeft(v, pos));
        }

        void IVFXProxy.SpawnDamageTextRight(Vector3 position, string value)
        {
            SpawnDamageText(position, value, (dt, v, pos) => dt.PlayDamageTextRight(v, pos));
        }
        
        public void SpawnPush(Vector3 position)
        {
            Wind wind = _vfxFactory.Get<Wind>().OrNull();
            if(wind.OrNull() == null) return;
            wind.Initialize(position);
            _vfxEntities.Add(wind);
        }
        
        async UniTask IVFXProxy.SpawnMeteor(Vector3 position, Vector3 destination, MeteorConfig config, string key, float damageToApply)
        {
            Meteor meteor =  await _vfxFactory.GetAsync<Meteor>(key);
            Meteor meteorObject = meteor.OrNull();
            if(meteorObject.OrNull() == null) return;
            meteorObject.Initialize(position, destination, config, this, damageToApply);
            _vfxEntities.Add(meteorObject);
        }
        
        public void SpawnRecover(Vector3 position, Sprite sprite, string value)
        {
            FoodRecover foodRecover = _vfxFactory.Get<FoodRecover>().OrNull();
            if(foodRecover.OrNull() == null) return;
            foodRecover.Initialize(position, sprite, value);
            foodRecover.Play();
            _vfxEntities.Add(foodRecover);
        }

        public void SpawnBattlePassLoot(Vector3 position)
        {
            BattlePassLootPoint loot = _vfxFactory.Get<BattlePassLootPoint>().OrNull();
            if(loot.OrNull() == null) return;
            loot.Initialize(position);
            loot.Play();
            _vfxEntities.Add(loot);
        }

        async UniTask IVFXProxy.SpawnMeteorExplosion(string key, Vector3 position)
        {
            MeteorExplosion explosion = await _vfxFactory.GetAsync<MeteorExplosion>(key);
            MeteorExplosion explosionObject = explosion.OrNull();
            if(explosionObject.OrNull() == null) return;
            explosionObject.Initialize(position);
            _vfxEntities.Add(explosionObject);
        }

        void IVFXProxy.SpawnUnitVfx(Vector3 position)
        {
            UnitExplosion explosion =  _vfxFactory.Get<UnitExplosion>().OrNull();
            if(explosion.OrNull() == null) return;
            explosion.Initialize(position);
            _vfxEntities.Add(explosion);
            
            UnitBlot unitBlot = _vfxFactory.Get<UnitBlot>().OrNull();
            if(unitBlot.OrNull() == null) return;
            unitBlot.Initialize(position);
            _vfxEntities.Add(unitBlot);
        }

        public void SpawnBasesSmoke(Vector3 basePosition)
        {
            var baseSmoke =  _vfxFactory.Get<BaseSmoke>().OrNull();
            if(baseSmoke.OrNull() == null) return;
            baseSmoke.Initialize(basePosition);
            _vfxEntities.Add(baseSmoke);
        }

        public void Cleanup()
        {
            _vfxEntities.Clear();
            _logger.Log("VFX_SPAWNER CLEANUP", DebugStatus.Info);
        }
    }
}