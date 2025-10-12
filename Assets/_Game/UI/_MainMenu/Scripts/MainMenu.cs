using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Temp;
using _Game.UI._Hud;
using _Game.UI._MainMenu.State;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.Scripts
{
    public class MainMenu :
        IMenuScreen,
        IGameScreenEvents<IMenuScreen>,
        IDisposable
    {
        public event Action<IMenuScreen> ScreenOpened;
        public event Action<IMenuScreen> InfoChanged;
        public event Action<IMenuScreen> RequiresAttention;
        public event Action<IMenuScreen> ScreenClosed;
        public event Action<IMenuScreen, bool> ActiveChanged;
        public event Action<IMenuScreen> ScreenDisposed;

        public bool IsReviewed => true;
        public bool NeedAttention => false;

        private readonly IAudioService _audioService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly ITutorialManager _tutorialManager;
        private readonly IUINotifier _uiNotifier;
        private readonly IParticleAttractorRegistry _registry;
        private readonly Curtain _curtain;
        private readonly MenuStateFactory _factory;
        private readonly IMyLogger _logger;

        private LocalStateMachine _menuStateMachine;
        public MainMenuScreen Screen { get; set; }

        private bool IsDungeonUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.Dugneons);
        private bool IsUpgradesUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.UpgradesScreen);
        private bool IsEvolutionAScreenUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.EvolutionScreen);
        private bool IsBattleUnlocked => true;
        private bool IsCardsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.Cards);
        private bool IsShopUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.Shop);
        private bool IsSkillsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.Skills);

        private Action _lastStateEnter;

        public MainMenu(
            IAudioService audioService,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IMyLogger logger,
            IUINotifier uiNotifier,
            Curtain curtain,
            MenuStateFactory menuStateFactory,
            IParticleAttractorRegistry registry)
        {
            _registry = registry;
            _audioService = audioService;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _logger = logger;
            _curtain = curtain;
            _uiNotifier = uiNotifier;

            _factory = menuStateFactory;

            _uiNotifier.RegisterScreen(this, this);
        }

        public async void OnScreenOpened()
        {
            if (Screen != null)
            {
                _tutorialManager.Register(Screen.CardsTutorialStep);
                _tutorialManager.Register(Screen.UpgradeTutorialStep);
                _tutorialManager.Register(Screen.EvolveTutorialStep);
                _tutorialManager.Register(Screen.SkillsTutorialStep);
                
                if (IsUpgradesUnlocked) Screen.UpgradeTutorialStep.ShowStep(0.5f);
                if (IsEvolutionAScreenUnlocked) Screen.EvolveTutorialStep.ShowStep(0.5f);
                if (IsCardsUnlocked) Screen.CardsTutorialStep.ShowStep(0.5f);
                if (IsSkillsUnlocked) Screen.SkillsTutorialStep.ShowStep(0.5f);

                Unsubscribe();
                Subscribe();

                await InitStateMachineAsync();

                InitButtons();

                EnterLastState();

                _registry.Register(Screen.SkillAttractorWrapper.ParticleAttractableType, Screen.SkillAttractorWrapper.Attractor);
                Screen.SkillAttractorWrapper.Attractor.onAttracted.AddListener(OnSkillPotionsAttracted);


                ScreenOpened?.Invoke(this);
            }
        }

        private void OnSkillPotionsAttracted() =>
            _audioService.PlayVfxAttractSound(AttractableParticleType.SkillPotions);

        private void EnterLastState()
        {
            if (_lastStateEnter != null)
            {
                _lastStateEnter.Invoke();
            }
            else
            {
                EnterBattleState();
            }

            HideCurtain();
        }

        public void OnScreenClosed()
        {
            _registry.TryDeregister(Screen.SkillAttractorWrapper.ParticleAttractableType);
            Screen.SkillAttractorWrapper.Attractor.onAttracted.RemoveListener(OnSkillPotionsAttracted);

            Screen.CardsTutorialStep.CancelStep();
            Screen.UpgradeTutorialStep.CancelStep();
            Screen.EvolveTutorialStep.CancelStep();
            Screen.SkillsTutorialStep.CancelStep();

            _tutorialManager.UnRegister(Screen.CardsTutorialStep);
            _tutorialManager.UnRegister(Screen.UpgradeTutorialStep);
            _tutorialManager.UnRegister(Screen.EvolveTutorialStep);
            _tutorialManager.UnRegister(Screen.SkillsTutorialStep);

            _uiNotifier.UnregisterPin(typeof(IStartBattleScreen));
            _uiNotifier.UnregisterPin(typeof(IUpgradeUnitsScreen));
            _uiNotifier.UnregisterPin(typeof(IGeneralCardsScreen));
            _uiNotifier.UnregisterPin(typeof(IShopScreen));
            _uiNotifier.UnregisterPin(typeof(IDungeonScreen));

            HideCurtain();
            Unsubscribe();

            _menuStateMachine.SetActive(false);

            ScreenClosed?.Invoke(this);
        }

        public void ShowEvolveStep()
        {
            if (IsEvolutionAScreenUnlocked && Screen.OrNull() != null)
            {
                Screen.EvolveTutorialStep.ShowStep(0.5f);
            }
        }

        public void ShowUpgradesStep()
        {
            if (IsUpgradesUnlocked && Screen.OrNull() != null)
            {
                Screen.UpgradeTutorialStep.ShowStep(0.5f);
            }
        }

        public void ShowCardsStep()
        {
            if (IsCardsUnlocked && Screen.OrNull() != null)
            {
                Screen.CardsTutorialStep.ShowStep(0.5f);
            }
        }

        public void ShowSkillsStep()
        {
            if (IsSkillsUnlocked && Screen.OrNull() != null)
            {
                Screen.SkillsTutorialStep.ShowStep(0.5f);
            }
        }

        public void CancelEvolveStep()
        {
            if (IsEvolutionAScreenUnlocked && Screen.OrNull() != null)
            {
                Screen.EvolveTutorialStep.CancelStep();
            }
        }

        public void CancelUpgradesStep()
        {
            if (Screen.OrNull() != null)
            {
                Screen.UpgradeTutorialStep.CancelStep();
            }
        }

        public void CancelCardsStep()
        {
            if (IsCardsUnlocked && Screen.OrNull() != null)
            {
                Screen.CardsTutorialStep.CancelStep();
            }
        }

        public void CancelSkillsStep()
        {
            if (IsSkillsUnlocked && Screen.OrNull() != null)
            {
                Screen.SkillsTutorialStep.CancelStep();
            }
        }

        private async UniTask InitStateMachineAsync()
        {
            if (_menuStateMachine != null)
            {
                _menuStateMachine.SetActive(true);
                return;
            }

            _menuStateMachine = new LocalStateMachine();

            BattleState battleState = _factory.CreateState<BattleState>(this, Screen.BattleButton);
            MenuUpgradesState menuUpgradesState = _factory.CreateState<MenuUpgradesState>(this, Screen.UpgradesButton);
            ShopState shopState = _factory.CreateState<ShopState>(this, Screen.ShopButton);
            GeneralCardsState generalCardsState = _factory.CreateState<GeneralCardsState>(this, Screen.CardsButton);
            DungeonsState dungeonsState = _factory.CreateState<DungeonsState>(this, Screen.DungeonButton);

            _menuStateMachine.AddState(battleState);
            _menuStateMachine.AddState(menuUpgradesState);
            _menuStateMachine.AddState(shopState);
            _menuStateMachine.AddState(generalCardsState);
            _menuStateMachine.AddState(dungeonsState);

            await _menuStateMachine.InitializeAsync();
        }

        private void InitButtons()
        {
            SetupMenuButton(Screen.DungeonButton, !IsDungeonUnlocked);
            SetupMenuButton(Screen.BattleButton, !IsBattleUnlocked);
            SetupMenuButton(Screen.UpgradesButton, !IsUpgradesUnlocked);
            SetupMenuButton(Screen.CardsButton, !IsCardsUnlocked);
            SetupMenuButton(Screen.ShopButton, !IsShopUnlocked);

            if (IsDungeonUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IDungeonScreen), Screen.DungeonButton.Pin);
            }
            else Screen.DungeonButton.SetupPin(false);

            if (IsBattleUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IStartBattleScreen), Screen.BattleButton.Pin);
            }
            else Screen.BattleButton.SetupPin(false);

            if (IsUpgradesUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IUpgradeUnitsScreen), Screen.UpgradesButton.Pin);
            }
            else Screen.UpgradesButton.SetupPin(false);

            if (IsCardsUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IGeneralCardsScreen), Screen.CardsButton.Pin);
            }
            else Screen.CardsButton.SetupPin(false);

            if (IsShopUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IShopScreen), Screen.ShopButton.Pin);
            }
            else Screen.ShopButton.SetupPin(false);
        }

        private void SetupMenuButton(MenuButton button, bool isLocked)
        {
            button.SetLocked(isLocked);
            button.SetInteractable(!isLocked);
        }

        private void HideCurtain() => _curtain.Hide();
        private void ShowCurtain() => _curtain.Show();


        private void Subscribe()
        {
            GlobalEvents.OnInsufficientFunds += OnInsufficientFunds;

            Screen.BattleButton.ButtonClicked += OnBattleButtonClick;
            Screen.UpgradesButton.ButtonClicked += OnUpgradeButtonClick;
            Screen.CardsButton.ButtonClicked += OnCardsButtonClick;
            Screen.ShopButton.ButtonClicked += OnShopButtonClick;
            Screen.DungeonButton.ButtonClicked += OnDungeonClick;

            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
        }

        private void Unsubscribe()
        {
            GlobalEvents.OnInsufficientFunds -= OnInsufficientFunds;

            Screen.BattleButton.ButtonClicked -= OnBattleButtonClick;
            Screen.UpgradesButton.ButtonClicked -= OnUpgradeButtonClick;
            Screen.CardsButton.ButtonClicked -= OnCardsButtonClick;
            Screen.ShopButton.ButtonClicked -= OnShopButtonClick;
            Screen.DungeonButton.ButtonClicked -= OnDungeonClick;

            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            switch (feature)
            {
                case Feature.Dugneons:
                    HandleDungeonsUnlock();
                    break;
                case Feature.Cards:
                    ShowCardsStep();
                    break;
                case Feature.Skills:
                    HandleSkillsUnlocked();
                    break;
            }
        }

        private void HandleSkillsUnlocked()
        {
            if (Screen.OrNull() != null)
            {
                _tutorialManager.Register(Screen.SkillsTutorialStep);
                ShowSkillsStep();
            }
        }

        private void HandleDungeonsUnlock()
        {
            SetupMenuButton(Screen.DungeonButton, !IsDungeonUnlocked);

            if (IsDungeonUnlocked)
            {
                _uiNotifier.RegisterPin(typeof(IDungeonScreen), Screen.DungeonButton.Pin);
            }
            else Screen.DungeonButton.SetupPin(false);
        }


        private void OnInsufficientFunds() => OnShopButtonClick();


        private void CacheCurrentState(Action stateEnterAction)
        {
            _lastStateEnter = stateEnterAction;
        }

        private void OnBattleButtonClick()
        {
            PlayButtonSound();
            EnterBattleState();
        }

        private void EnterBattleState()
        {
            CacheCurrentState(EnterBattleState);
            _menuStateMachine.Enter<BattleState>();
            HideCurtain();
        }

        private void OnUpgradeButtonClick() => EnterMenuUpgradesState();


        private void EnterMenuUpgradesState()
        {
            CacheCurrentState(EnterMenuUpgradesState);
            _menuStateMachine.Enter<MenuUpgradesState>();
            ShowCurtain();
        }

        private void OnShopButtonClick()
        {
            PlayButtonSound();
            EnterShopState();
        }

        private void EnterShopState()
        {
            CacheCurrentState(EnterShopState);
            _menuStateMachine.Enter<ShopState>();
            ShowCurtain();
        }

        private void OnCardsButtonClick() => EnterGeneralCardsState();


        private void EnterGeneralCardsState()
        {
            CacheCurrentState(EnterGeneralCardsState);
            _menuStateMachine.Enter<GeneralCardsState>();
            ShowCurtain();
        }

        private void OnDungeonClick()
        {
            PlayButtonSound();
            EnterDungeonsState();
        }

        private void EnterDungeonsState()
        {
            CacheCurrentState(EnterDungeonsState);
            _menuStateMachine.Enter<DungeonsState>();
            ShowCurtain();
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();

        void IDisposable.Dispose()
        {
            _menuStateMachine.Cleanup();
            HideCurtain();
            _uiNotifier.UnregisterScreen(this);
        }

        public void SetButtonHighlighted(MenuButtonType buttonType, bool isHighLighted)
        {
            if(Screen.OrNull() == null) return;
            Screen.SetButtonHighlighted(buttonType, isHighLighted);
        }
    }
}