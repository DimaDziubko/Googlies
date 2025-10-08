using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;
using _Game.LiveopsCore;
using _Game.UI._EvolveScreen.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI.Common.Scripts;
using _Game.UI._EvolutionScreen;
using _Game.UI.Global;
using _Game.UI.Settings.Scripts;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using UnityUtils;
using Zenject;

namespace _Game.UI._StartBattleScreen.Scripts
{
    public class StartBattleScreenPresenter :
        IStartBattleScreenPresenter,
        IGameScreenEvents<IStartBattleScreen>,
        IStartBattleScreen, IDisposable
    {
        public event Action<IStartBattleScreen> ScreenOpened;
        public event Action<IStartBattleScreen> InfoChanged;
        public event Action<IStartBattleScreen> RequiresAttention;
        public event Action<IStartBattleScreen> ScreenClosed;
        public event Action<IStartBattleScreen, bool> ActiveChanged;
        public event Action<IStartBattleScreen> ScreenDisposed;

        public StartBattleScreen Screen { get; set; }

        [ShowInInspector, ReadOnly]
        public bool IsReviewed => true;

        [ShowInInspector, ReadOnly]
        public bool NeedAttention => false;

        public string Info => $"Battle {_battleNavigator.CurrentBattleIdx + 1}";

        private readonly IGameManager _gameManager;
        private readonly IMyLogger _logger;
        private readonly ISettingsPopupProvider _settingsPopupProvider;
        private readonly IBattleNavigator _battleNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IAudioService _audioService;
        private readonly IUINotifier _uiNotifier;
        private readonly IBalancySDKService _balancy;
        private readonly IGameEventScheduler _gameEventScheduler;
        private EvolutionPresenter _evolutionPresenter;
        private readonly IEvolveScreenProvider _evolveProvider;
        private CurrencyBank _bank;

        private bool _isOpened;

        public StartBattleScreenPresenter(
            IAudioService audioService,
            IGameManager gameManager,
            IMyLogger logger,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            IUINotifier uiNotifier,
            CurrencyBank bank,
            IBalancySDKService balancy,
            IGameEventScheduler gameEventScheduler,
            IEvolveScreenProvider evolveProvider
            )
        {
            _balancy = balancy;
            _bank = bank;
            _gameManager = gameManager;
            _logger = logger;
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _audioService = audioService;
            _uiNotifier = uiNotifier;
            _gameEventScheduler = gameEventScheduler;
            _evolveProvider = evolveProvider;

            _uiNotifier.RegisterScreen(this, this);
        }

        void IStartBattleScreenPresenter.OnScreenOpened()
        {
            if (Screen.OrNull() != null)
            {
                _isOpened = true;
                Subscribe();
                OnTimelineChanged();
                OnBalancyInitialized();

                ScreenOpened?.Invoke(this);
            }
        }

        void IStartBattleScreenPresenter.OnScreenClosed()
        {
            _isOpened = false;
            Unsubscribe();
            ScreenClosed?.Invoke(this);
        }

        void IStartBattleScreenPresenter.OnScreenDispose()
        {
            _isOpened = false;
            Unsubscribe();
            ScreenDisposed?.Invoke(this);
        }

        void IStartBattleScreenPresenter.OnActiveChanged(bool isActive)
        {
            ActiveChanged?.Invoke(this, isActive);
        }

        private void Subscribe()
        {
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _battleNavigator.BattleChanged += OnBattleChanged;

            Screen.StartClicked += OnStartBattleClicked;
            Screen.EvolutionClicked += OnEvolutionButtonClicked;
            Screen.NextBattleClicked += OnNextBattleClicked;
            Screen.PreviousBattleClicked += OnPreviousBattleClicked;

            Screen.CheatPanel.NextTimelineClicked += OnNextTimelineClicked;
            Screen.CheatPanel.PreviousTimelineClicked += OnPreviousTimelineClicked;

            Screen.CheatPanel.NextAgeClicked += OnNextAgeClicked;
            Screen.CheatPanel.PreviousAgeClicked += OnPreviousAgeClicked;

            Screen.CheatPanel.NextBattleClicked += OnForceNextBattleClicked;
            Screen.CheatPanel.PreviousBattleClicked += OnPreviousBattleClicked;

            Screen.CheatPanel.CoinsButtonClicked += OnCoinsBtnClicked;
            Screen.CheatPanel.GemsButtonClicked += OnGemsBtnClicked;

            Screen.CheatPanel.AllBattlesWonClicked += OnAllBattlesWonClicked;
            Screen.CheatPanel.SkillPetPotionButtonClicked += OnSkillPetPotionButtonClicked;

            Screen.CheatPanel.BPPointsButtonClicked += OnBpPointsButtonClicked;

            _balancy.Initialized += OnBalancyInitialized;
            _gameEventScheduler.Initialized += OnBalancyInitialized;
        }

        private void Unsubscribe()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _battleNavigator.BattleChanged -= OnBattleChanged;

            Screen.StartClicked -= OnStartBattleClicked;
            Screen.EvolutionClicked -= OnEvolutionButtonClicked;
            Screen.NextBattleClicked -= OnNextBattleClicked;
            Screen.PreviousBattleClicked -= OnPreviousBattleClicked;

            Screen.CheatPanel.NextTimelineClicked -= OnNextTimelineClicked;
            Screen.CheatPanel.PreviousTimelineClicked -= OnPreviousTimelineClicked;

            Screen.CheatPanel.NextAgeClicked -= OnNextAgeClicked;
            Screen.CheatPanel.PreviousAgeClicked -= OnPreviousAgeClicked;

            Screen.CheatPanel.NextBattleClicked -= OnForceNextBattleClicked;
            Screen.CheatPanel.PreviousBattleClicked -= OnPreviousBattleClicked;

            Screen.CheatPanel.CoinsButtonClicked -= OnCoinsBtnClicked;
            Screen.CheatPanel.GemsButtonClicked -= OnGemsBtnClicked;

            Screen.CheatPanel.AllBattlesWonClicked -= OnAllBattlesWonClicked;
            Screen.CheatPanel.SkillPetPotionButtonClicked -= OnSkillPetPotionButtonClicked;

            Screen.CheatPanel.BPPointsButtonClicked -= OnBpPointsButtonClicked;

            _balancy.Initialized -= OnBalancyInitialized;
            _gameEventScheduler.Initialized -= OnBalancyInitialized;
        }

        private void OnAllBattlesWonClicked()
        {
            _audioService.PlayButtonSound();
            _battleNavigator.SetAllBattlesWon();
        }

        private void OnBalancyInitialized()
        {
            bool isInitialized = _balancy.IsInitialized;
            var profile = _balancy.GetProfile();

            string color = (isInitialized && profile != null) ? "green" : "red";

            Screen.CheatPanel.SetBalancyInfo(
                $"Balancy\n" +
                $"Initialized: <color={color}>{isInitialized}</color>\n" +
                $"Profile: <color={color}>{profile != null}</color>\n" +
                $"Scheduler: <color={color}>{_gameEventScheduler.IsInitialized}</color>");

            _logger.Log("START BATTLE SCREEN ON BALANCY INITIALIZED", DebugStatus.Info);
        }

        private void OnSkillPetPotionButtonClicked()
        {
            _audioService.PlayButtonSound();
            _bank.Add(new[] { new CurrencyData() { Type = CurrencyType.SkillPotion, Amount = 100 } });
        }

        private void OnBpPointsButtonClicked()
        {
            _audioService.PlayButtonSound();
            _bank.Add(new[] { new CurrencyData() { Type = CurrencyType.LeaderPassPoint, Amount = 10 } });
        }

        private void OnGemsBtnClicked()
        {
            _audioService.PlayButtonSound();
            _bank.Add(new[] { new CurrencyData() { Type = CurrencyType.Gems, Amount = 10_000 }, });
        }

        private void OnCoinsBtnClicked()
        {
            _audioService.PlayButtonSound();
            _bank.Add(new[] { new CurrencyData() { Type = CurrencyType.Coins, Amount = 10_000_000 }, });
        }


        private void OnTimelineChanged()
        {
            Screen.CheatPanel.SetTimeline($"Timeline {_timelineNavigator.CurrentTimelineNumber}");
            OnAgeChanged();

            Screen.CheatPanel.SetNextTimelineBtnInteractable(true);
            Screen.CheatPanel.SetPreviousTimelineBtnInteractable(_timelineNavigator.CurrentTimelineId > 0);

            InfoChanged?.Invoke(this);
        }

        private void OnAgeChanged()
        {
            Screen.CheatPanel.SetAge($"Age {_ageNavigator.CurrentIdx + 1}");
            OnBattleChanged();

            Screen.CheatPanel.SetNextAgeBtnInteractable(_ageNavigator.IsNextAge());
            Screen.CheatPanel.SetPreviousAgeBtnInteractable(_ageNavigator.IsPreviousAge());

            InfoChanged?.Invoke(this);
        }

        private void OnBattleChanged()
        {
            Screen.CheatPanel.SetBattle($"Battle {_battleNavigator.CurrentBattleIdx + 1}");

            Screen.CheatPanel.SetNextBattleBtnInteractable(_battleNavigator.IsNextBattle());
            Screen.CheatPanel.SetPreviousBattleBtnInteractable(_battleNavigator.IsPreviousBattle());

            Screen.SetStartBattleBtnInteractable(true);

            Screen.SetNextBattleBtnInteractable(_battleNavigator.CanMoveToNextBattle());
            Screen.SetPreviousBattleBtnInteractable(_battleNavigator.IsPreviousBattle());

            Screen.SetNextBattleBtnActive(_battleNavigator.CanMoveToNextBattle());
            Screen.SetPreviousBattleBtnActive(_battleNavigator.IsPreviousBattle());

            InfoChanged?.Invoke(this);
        }

        private void OnPreviousAgeClicked()
        {
            _ageNavigator.MoveToPreviousAge();
            _audioService.PlayButtonSound();
        }

        private void OnNextAgeClicked()
        {
            _ageNavigator.MoveToNextAge();
            _audioService.PlayButtonSound();
        }

        private void OnPreviousTimelineClicked()
        {
            _timelineNavigator.MoveToPreviousTimeline();
            _audioService.PlayButtonSound();
        }

        private void OnNextTimelineClicked()
        {
            _timelineNavigator.MoveToNextTimeline();
            _audioService.PlayButtonSound();
        }

        private void OnStartBattleClicked()
        {
            _gameManager.StartBattle();
            if (_gameManager.IsPaused) _gameManager.SetPaused(false);
            _audioService.PlayStartBattleSound();
        }

        private void OnEvolutionButtonClicked()
        {
            //Screen.EvolutionStep.CompleteStep();

            //_localStateMachine.Enter<EvolutionState>();

            if (_evolutionPresenter == null)
                _evolutionPresenter = new EvolutionPresenter(_evolveProvider, _logger);

            _evolutionPresenter.ShowScreen().Forget();
            PlayButtonSound();
        }

        private void OnForceNextBattleClicked()
        {
            _battleNavigator.ForceMoveToNextBattle();
            PlayButtonSound();
        }

        private void OnNextBattleClicked()
        {
            _battleNavigator.MoveToNextBattle();
            PlayButtonSound();
        }

        private void OnPreviousBattleClicked()
        {
            _battleNavigator.MoveToPreviousBattle();
            PlayButtonSound();
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();

        void IDisposable.Dispose()
        {
            Unsubscribe();
            _uiNotifier.UnregisterScreen(this);
        }
    }
}