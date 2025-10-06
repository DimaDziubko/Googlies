using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI._Skills.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityUtils;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreenPresenter :
        IGeneralCardsScreenPresenter,
        IGameScreenEvents<IGeneralCardsScreen>,
        IGeneralCardsScreen,
        IGameScreenListener<ICardsScreen>,
        IGameScreenListener<IMenuScreen>,
        IGameScreenListener<ISkillsScreen>,
        IDisposable
    {
        public event Action<IGeneralCardsScreen> ScreenOpened;
        public event Action<IGeneralCardsScreen> InfoChanged;
        public event Action<IGeneralCardsScreen> RequiresAttention;
        public event Action<IGeneralCardsScreen> ScreenClosed;
        public event Action<IGeneralCardsScreen, bool> ActiveChanged;
        public event Action<IGeneralCardsScreen> ScreenDisposed;

        public GeneralCardsScreen Screen { get; set; }
        public QuickBoostInfoPresenter QuickBoostInfoPresenter => _presenter;

        private readonly IUINotifier _uiNotifier;
        private readonly ICardsScreenProvider _cardsScreenProvider;
        private readonly ISkillsScreenProvider _skillsScreenProvider;
        private readonly IAudioService _audioService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IMyLogger _logger;
        private readonly IFeatureConfigRepository _config;
        private readonly ITutorialManager _tutorialManager;

        private readonly QuickBoostInfoPresenter.Factory _factory;

        private LocalStateMachine _localStateMachine;
        private QuickBoostInfoPresenter _presenter;


        [ShowInInspector, ReadOnly]
        public bool IsReviewed => _aggregator.IsReviewed;

        [ShowInInspector, ReadOnly]
        public bool NeedAttention => _aggregator.NeedAttention;

        private readonly ScreenStateAggregator _aggregator;

        private bool IsSkillsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.Skills);

        public GeneralCardsScreenPresenter(
            QuickBoostInfoPresenter.Factory factory,
            IFeatureUnlockSystem featureUnlockSystem,
            IUINotifier uiNotifier,
            ICardsScreenProvider cardsScreenProvider,
            IAudioService audioService,
            IMyLogger logger,
            IConfigRepository configRepository,
            ISkillsScreenProvider skillsScreenProvider,
            ITutorialManager tutorialManager
            )
        {
            _tutorialManager = tutorialManager;
            _config = configRepository.FeatureConfigRepository;
            _factory = factory;
            _featureUnlockSystem = featureUnlockSystem;
            _uiNotifier = uiNotifier;
            _cardsScreenProvider = cardsScreenProvider;
            _audioService = audioService;
            _logger = logger;
            _skillsScreenProvider = skillsScreenProvider;

            _aggregator = new ScreenStateAggregator();

            _uiNotifier.RegisterScreen(this, this);
        }

        async void IGeneralCardsScreenPresenter.OnGeneralCardsScreenOpened()
        {
            if (Screen.OrNull() != null)
            {
                _tutorialManager.Register(Screen.SkillsStep);
                TryToShowSkillsStep();

                Unsubscribe();
                Subscribe();
                await InitStateMachine();
                InitButtons();
                InitQuickBoostInfo();
                OnCardsButtonClicked();

                ScreenOpened?.Invoke(this);
            }
        }

        void IGeneralCardsScreenPresenter.OnGeneralCardsScreenClosed()
        {
            CancelSkillsStep();

            _tutorialManager.UnRegister(Screen.SkillsStep);

            _presenter?.Dispose();
            _localStateMachine?.Exit();
            _uiNotifier.UnregisterPin(typeof(ICardsScreen));
            _uiNotifier.UnregisterPin(typeof(ISkillsScreen));

            Unsubscribe();

            ScreenClosed?.Invoke(this);
        }

        void IGeneralCardsScreenPresenter.OnScreenDispose()
        {
            _localStateMachine?.Cleanup();
            _localStateMachine = null;
            _presenter?.Dispose();
            Unsubscribe();
        }

        void IGeneralCardsScreenPresenter.OnScreenActiveChanged(bool isActive) =>
            _localStateMachine?.SetActive(isActive);

        void IDisposable.Dispose() =>
            _uiNotifier.UnregisterScreen(this);

        private void InitButtons()
        {
            Screen.CardBtn.UnHighlight();
            //Screen.CardBtn.SetLocked(false);
            Screen.CardBtn.SetInteractable(true);
            _uiNotifier.RegisterPin(typeof(ICardsScreen), Screen.CardBtn.Pin);

            Screen.SkillsBtn.UnHighlight();
            //Screen.SkillsBtn.SetLockedText($"Timeline {_config.SkillRequiredTimeline}");
            //Screen.SkillsBtn.SetLocked(!IsSkillsUnlocked);
            Screen.SkillsBtn.SetInteractable(IsSkillsUnlocked);
            if (IsSkillsUnlocked)
                _uiNotifier.RegisterPin(typeof(ISkillsScreen), Screen.SkillsBtn.Pin);
            else
                Screen.SkillsBtn.SetupPin(false);

            Screen.RunesBtn.UnHighlight();
            //Screen.RunesBtn.SetLocked(true);
            Screen.RunesBtn.SetInteractable(false);
            Screen.RunesBtn.SetupPin(false);
            //Screen.RunesBtn.ButtonClicked += OnRunesButtonClicked;

            Screen.HeroesBtn.UnHighlight();
            //Screen.HeroesBtn.SetLocked(true);
            Screen.HeroesBtn.SetInteractable(false);
            Screen.HeroesBtn.SetupPin(false);
            //Screen.HeroesBtn.ButtonClicked += OnHeroesButtonClicked;
        }

        private void OnCardsButtonClicked()
        {
            _audioService.PlayButtonSound();
            _localStateMachine.Enter<CardsState>();
            Screen.QuickBoostInfoPanel.gameObject.SetActive(true);
        }

        private void OnSkillsButtonClicked()
        {
            _audioService.PlayButtonSound();
            _localStateMachine.Enter<SkillsState>();
            Screen.QuickBoostInfoPanel.gameObject.SetActive(false);
        }

        private void InitQuickBoostInfo()
        {
            if (_presenter != null)
            {
                _presenter.Dispose();
                _presenter.SetView(Screen.QuickBoostInfoPanel);
            }
            else
            {
                _presenter = _factory.Create(Screen.QuickBoostInfoPanel);
            }

            _presenter.SetMainSource(BoostSource.Cards);
            _presenter.SetSubSource(BoostSource.Total);
            _presenter.Initialize();
        }

        private void Unsubscribe()
        {
            if (Screen != null)
            {
                Screen.CardBtn.ButtonClicked -= OnCardsButtonClicked;
                Screen.SkillsBtn.ButtonClicked -= OnSkillsButtonClicked;
            }
        }

        private void Subscribe()
        {
            Screen.CardBtn.ButtonClicked += OnCardsButtonClicked;
            Screen.SkillsBtn.ButtonClicked += OnSkillsButtonClicked;
        }

        private async UniTask InitStateMachine()
        {
            if (_localStateMachine != null)
            {
                _localStateMachine.SetActive(true);
                return;
            }

            _localStateMachine = new LocalStateMachine();

            CardsState cardsState = new CardsState(_cardsScreenProvider, this, _logger);
            SkillsState skillsState = new SkillsState(_skillsScreenProvider, this, _logger);

            _localStateMachine.AddState(cardsState);
            _localStateMachine.AddState(skillsState);

            await _localStateMachine.InitializeAsync();
        }

        public void TryToShowSkillsStep()
        {
            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills) && Screen.OrNull() != null)
            {
                Screen.SkillsStep.ShowStep();
            }
        }

        public void CompleteSkillsStep()
        {
            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills) && Screen.OrNull() != null)
            {
                Screen.SkillsStep.CompleteStep();
            }
        }

        public void CancelSkillsStep()
        {
            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills) && Screen.OrNull() != null)
            {
                Screen.SkillsStep.CancelStep();
            }
        }

        void IGameScreenListener<ICardsScreen>.OnScreenOpened(ICardsScreen screen) => UpdateState(screen);
        void IGameScreenListener<ICardsScreen>.OnInfoChanged(ICardsScreen screen) { }
        void IGameScreenListener<ICardsScreen>.OnRequiresAttention(ICardsScreen screen) => UpdateState(screen);
        void IGameScreenListener<ICardsScreen>.OnScreenClosed(ICardsScreen screen) { }
        void IGameScreenListener<ICardsScreen>.OnScreenActiveChanged(ICardsScreen screen, bool isActive) { }
        void IGameScreenListener<ICardsScreen>.OnScreenDisposed(ICardsScreen screen) { }

        void IGameScreenListener<ISkillsScreen>.OnScreenOpened(ISkillsScreen screen) => UpdateState(screen);
        void IGameScreenListener<ISkillsScreen>.OnInfoChanged(ISkillsScreen screen) { }
        void IGameScreenListener<ISkillsScreen>.OnRequiresAttention(ISkillsScreen screen) => UpdateState(screen);
        void IGameScreenListener<ISkillsScreen>.OnScreenClosed(ISkillsScreen screen) { }
        void IGameScreenListener<ISkillsScreen>.OnScreenActiveChanged(ISkillsScreen screen, bool isActive) { }
        void IGameScreenListener<ISkillsScreen>.OnScreenDisposed(ISkillsScreen screen) { }

        private void UpdateState(IGameScreen screen)
        {
            if (_aggregator.UpdateState(screen))
            {
                RequiresAttention?.Invoke(this);
            }
        }

        public void HighlightCardBtn() => Screen.CardBtn.Highlight();
        public void UnHighlightCardBtn() => Screen.CardBtn.UnHighlight();
        public void HighlightSkillsBtn() => Screen?.SkillsBtn.Highlight();
        public void UnHighlightSkillsBtn() => Screen?.SkillsBtn.UnHighlight();

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}