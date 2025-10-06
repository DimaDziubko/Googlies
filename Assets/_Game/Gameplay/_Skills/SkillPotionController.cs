using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.UserState._State;
using Zenject;

namespace _Game.Gameplay._Skills
{
    public class SkillPotionController : 
        IInitializable,
        IDisposable
    {
        private readonly CurrencyBank _bank;
        private readonly ISkillConfigRepository _skillConfig;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        
        public SkillPotionController(
            CurrencyBank bank, 
            IConfigRepository skillConfig,
            IFeatureUnlockSystem featureUnlockSystem,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator)
        {
            _bank = bank;
            _skillConfig = skillConfig.SkillConfigRepository;
            _featureUnlockSystem = featureUnlockSystem;
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
        }

        void IInitializable.Initialize()
        {
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            _ageNavigator.AgeChanged += AgeChanged; 
            
        }

        void IDisposable.Dispose()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _ageNavigator.AgeChanged -= AgeChanged; 
        }

        private void OnTimelineChanged() => TrySkillPotions();

        private void AgeChanged() => TrySkillPotions();

        private void TrySkillPotions()
        {
            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills))
            {
                _bank.Add(_skillConfig.RewardPerEvolve);
            }
        }
    }
}