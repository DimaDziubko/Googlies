using System;
using System.Collections;
using _Game.Common;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common;
using _Game.UI._Skills.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._Skills
{
    public class MeteorsStrategy : 
        ISkillStrategy,
        IPauseListener,
        IStopGameListener
    {
        public event Action Activated;
        public event Action Completed;
        
        private readonly SkillModel _skillModel;
        private readonly IBattleField _battleField;
        private readonly IAudioService _audioService;
        private readonly IUnitDataProvider _unitDataProvider;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IMyLogger _logger;

        private Coroutine _coroutine;

        private bool _isPaused = false;


        public MeteorsStrategy(
            SkillModel skillModel, 
            IBattleField battlefield, 
            IAudioService audioService,
            IUnitDataProvider unitDataProvider,
            ICoroutineRunner coroutineRunner,
            IMyLogger logger)
        {
            _skillModel = skillModel;
            _battleField = battlefield;
            _audioService = audioService;
            _unitDataProvider = unitDataProvider;
            _coroutineRunner = coroutineRunner;
            _logger = logger;
        }

        bool ISkillStrategy.Execute()
        {
            MeteorsSkillConfig meteorsConfig = _skillModel.GetSpecificConfig<MeteorsSkillConfig>();
            
            if (meteorsConfig != null)
            {
                Activated?.Invoke();
                PlaySfx();
                _coroutine = _coroutineRunner.StartCoroutine(SpawnMeteorCoroutine(meteorsConfig));
                Completed?.Invoke();
                return true;
                
            }
            return false;
        }

        void ISkillStrategy.Interrupt() { }

        private IEnumerator SpawnMeteorCoroutine(MeteorsSkillConfig meteorsConfig)
        {
            var startingUnitData = _unitDataProvider.GetDecoratedUnitData(Faction.Player, UnitType.Light, Skin.Ally);
            var meteorDamageFactor = meteorsConfig.GetValue(_skillModel.Skill.Level) + 1;
            var meteorDamage = startingUnitData.WeaponData.Damage * meteorDamageFactor;
            
            for (int i = 0; i < meteorsConfig.MeteorsAmount; i++)
            {
                while (_isPaused)
                {
                    yield return null;
                }
                
                SpawnMeteor(meteorsConfig.MeteorConfig, meteorDamage);
                yield return new WaitForSeconds(meteorsConfig.SpawnDelay);
            }
        }

        private void SpawnMeteor(MeteorConfig meteorConfig, float damage)
        {
            Vector3 offset = new Vector3(-3f, 0, 0);
            Vector3 topPoint = _battleField.Settings.GetRandomTopPoint();
            Vector3 destination = _battleField.Settings.GetRandomPointOnFieldWithBaseOffset();
            Vector3 spawnPoint = new Vector3(destination.x + offset.x, topPoint.y +topPoint.y, 0);
            
            _battleField.VFXProxy.SpawnMeteor(spawnPoint, destination, meteorConfig, meteorConfig.PrefabKey, damage).Forget();
        }
        
        private void PlaySfx()
        {
            var clip = _skillModel.Sfx;
            
            if (clip != null)
            {
                _audioService.PlayOneShot(_skillModel.Sfx);
            }
        }

        void IPauseListener.SetPaused(bool isPaused) => _isPaused = isPaused;

        void IStopGameListener.OnStopBattle()
        {
            if (_coroutine != null)
            {
                _coroutineRunner.StopCoroutine(_coroutine);
            }
        }
    }
}