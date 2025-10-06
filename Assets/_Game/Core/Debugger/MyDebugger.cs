using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core.Boosts;
using _Game.Core.Data;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.LiveopsCore;
using _Game.UI._ParticleAttractorSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Debugger
{
    public class MyDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        [Inject, ShowInInspector]
        private ParticleAttractorRegistry _registry;
        
        [Inject, ShowInInspector]
        private UserContainer _userContainer;
        
        [Inject, ShowInInspector]
        private AdsGemsPackService _adsGemsPackService;
        
        [Inject, ShowInInspector]
        private BoostContainer _boostContainer;
        
        [Inject, ShowInInspector]
        private IAPService _iasService;
        
        [Inject, ShowInInspector]
        private WarriorIconContainer _iconContainer;
        
        [Inject, ShowInInspector]
        private AgeIconContainer _ageIconContainer;
        
        [Inject, ShowInInspector]
        private AmbienceContainer _ambienceContainer;
        
        [Inject, ShowInInspector]
        private ShopIconsContainer _shopIconsContainer;
        
        [Inject, ShowInInspector]
        private SoundService _soundService;

        [Inject, ShowInInspector]
        private CurrencyBank _bank;
        
        [Inject, ShowInInspector]
        private TemporaryCurrencyBank _temporaryBank;
        
        [Inject, ShowInInspector]
        private TargetRegistry _targetRegistry;
        
        [Inject, ShowInInspector]
        private ParticleAttractorRegistry _particleAttractorRegistry;
        
        [Inject, ShowInInspector]
        private GameEventContainer _container;
        
        [Inject, ShowInInspector]
        private GameEventPendingRewardShower _shower;

        //[Inject, ShowInInspector]
        //private BattleField _battlefield;
        
        [Button]
        public void AddDailyProgress()
        {
            _userContainer.DailyTaskStateHandler.AddProgress(1);
        }
        
        [Button]
        public void AddSkillsPotion()
        {
            List<CurrencyData> skillsPetFeed = new List<CurrencyData>()
            {
                new()
                {
                    Amount = 10,
                    Source = ItemSource.None,
                    Type = CurrencyType.SkillPotion
                }
            };

            _bank.Add(skillsPetFeed);
        }
        
        [Button]
        public void AddLeaderPoints()
        {
            List<CurrencyData> leaderPoints = new List<CurrencyData>()
            {
                new()
                {
                    Amount = 100,
                    Source = ItemSource.None,
                    Type = CurrencyType.LeaderPassPoint
                }
            };

            _bank.Add(leaderPoints);
        }
        
        [Button]
        public void ResetLeaderPoints()
        {
            _bank.GetCell(CurrencyType.LeaderPassPoint).Change(0);
        }
#endif
    }
}