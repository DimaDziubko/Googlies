using System;
using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Skills;
using _Game.UI._GameplayUI.Scripts;
using Zenject;

namespace _Game.UI._Skills.Scripts
{
    public class SkillListPresenter :
        IInitializable, 
        IDisposable,
        IStartGameListener,
        IStopGameListener,
        IPauseListener
    {
        private readonly ISkillService _skillService;
        private readonly IAudioService _audioService;
        private readonly IUserContainer _userContainer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private readonly SkillStrategyFactory _skillStrategyFactory;

        private readonly List<SkillPresenter> _presenters = new();
        
        private readonly GameplayUI _gameplayUI;

        public SkillListPresenter(
            GameplayUI gameplayUI,
            ISkillService skillService,
            IAudioService audioService,
            SkillStrategyFactory skillStrategyFactory,
            IUserContainer userContainer,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _gameplayUI = gameplayUI;
            _skillService = skillService;
            _audioService = audioService;
            _skillStrategyFactory = skillStrategyFactory;
            _userContainer = userContainer;
            _featureUnlockSystem = featureUnlockSystem;
        }

        void IInitializable.Initialize()
        {
            foreach (var skillView in _gameplayUI.SkillViews)
            {
                var skillSlot = _skillService.SkillSlotContainer.GetSlot(skillView.Id);

                if (skillSlot != null)
                {
                    var presenter = new SkillPresenter(skillView, skillSlot, _audioService, _skillStrategyFactory, _userContainer);
                    _presenters.Add(presenter);
                }
            }
        }

        void IStartGameListener.OnStartBattle()
        {
            _gameplayUI.SetSkillPanelActive(_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills));
            
            foreach (var presenter in _presenters)
            {
                presenter.Initialize();
            }
        }

        void IStopGameListener.OnStopBattle()
        {
            foreach (var presenter in _presenters)
            {
                presenter.OnStopBattle();
                presenter.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }
            
            _presenters.Clear();
        }

        void IPauseListener.SetPaused(bool isPaused)
        {
            foreach (var presenter in _presenters)
            {
                presenter.SetPaused(isPaused);
            }
        }
    }
}