using _Game.Core._GameListenerComposite;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Skills;

namespace _Game.UI._Skills.Scripts
{
    public class SkillPresenter
    {
        private readonly SkillSocket _skillView;
        private readonly SkillSlot _skillSlot;
        private readonly SkillStrategyFactory _skillStrategyFactory;
        private readonly IUserContainer _userContainer;
        private readonly IAudioService _audioService;
        
        
        private ISkillStrategy _currentStrategy;
        private IPauseListener _pauseListener;
        private IStopGameListener _stopGameListener;
        
        private bool _isCompleted;
        private bool _isActivated;

        public SkillPresenter(
            SkillSocket skillView, 
            SkillSlot skillSlot,
            IAudioService audioService,
            SkillStrategyFactory skillStrategyFactory,
            IUserContainer userContainer)
        {
            _skillView = skillView;
            _skillSlot = skillSlot;
            _audioService = audioService;
            _skillStrategyFactory = skillStrategyFactory;
            _userContainer = userContainer;
        }

        public void Initialize()
        {
            _isCompleted = false;
            _isActivated = false;
            
            if (_skillSlot.IsLocked)
            {
                InitializeLocked(_skillView, _skillSlot);
            }
            else if(_skillSlot.IsEquipped)
            {
                InitializeEquipped(_skillView, _skillSlot);
            }
            else
            {
                InitializeUnequipped(_skillView, _skillSlot);
            }
        }
        
        private void InitializeLocked(SkillSocket skillView, SkillSlot skillSlot)
        {
            skillView.SetActive(false);
        }

        private void InitializeUnequipped(SkillSocket skillView, SkillSlot skillSlot)
        {
            skillView.SetActive(true);
            skillView.SetEquipped(false);
            _skillView.SmoothProgressController.SetActive(false);
        }

        private void InitializeEquipped(SkillSocket skillView, SkillSlot skillSlot)
        {
            skillView.SetActive(true);
            skillView.SetEquipped(true);
            _skillView.SetIcon(skillSlot.SkillModel.GetIcon());
            _skillView.SmoothProgressController.SetActive(false);
            _skillView.SetInteractable(true);
            
            _skillView.OnSkillClicked += OnSkillClicked;
            
            _currentStrategy = _skillStrategyFactory.GetStrategy(_skillSlot.SkillModel);
            
            _currentStrategy.Completed += OnCompleted;
            _currentStrategy.Activated += OnActivated;
        }

        private void OnCompleted()
        {
            _isCompleted = true;
            _skillView.SetInteractable(false);
            _skillView.SmoothProgressController.SetActive(false);
        }


        public void Dispose()
        {
            _skillView.OnSkillClicked -= OnSkillClicked;
            
            if (_currentStrategy != null)
            {
                _currentStrategy.Completed -= OnCompleted;
                _currentStrategy.Activated -= OnActivated;
            }
        }

        private void OnActivated()
        {
            _isActivated = true;
            _skillView.SmoothProgressController.SetActive(true);
        }

        private void OnSkillClicked()
        {
            _audioService.PlayButtonSound();

            if (!_isActivated)
            {
                if (_currentStrategy.Execute())
                {
                    if (_currentStrategy is IPauseListener strategy)
                    {
                        _pauseListener = strategy;
                    }

                    if (_currentStrategy is IStopGameListener listener)
                    {
                        _stopGameListener = listener;
                    }
                    
                    if (_skillSlot.SkillModel.IsCountdownNeeded)
                    {
                        _skillView.SmoothProgressController.SetProgressSmooth(1, 0, _skillSlot.SkillModel.Duration);
                    }
                }
            }
            else if (_skillSlot.SkillModel.IsInterruptible)
            {
                _currentStrategy.Interrupt();
                _skillView.SmoothProgressController.SetActive(false);
                _isActivated = false;
            }
        }

        public void SetPaused(bool isPaused)
        {
            _skillView.SetInteractable(!isPaused && !_isCompleted);
            _pauseListener?.SetPaused(isPaused);
        }

        public void OnStopBattle()
        {
            _stopGameListener?.OnStopBattle();
        }
    }
}