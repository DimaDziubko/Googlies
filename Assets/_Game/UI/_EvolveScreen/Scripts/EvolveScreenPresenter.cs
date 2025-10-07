using System;
using _Game.Core._GameInitializer;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveScreenPresenter :
        IEvolveScreenPresenter,
        IDisposable,
        IGameScreenEvents<IEvolveScreen>, 
        IEvolveScreen,
        IGameScreenListener<ITravelScreen>,
        IGameScreenListener<IMenuScreen>
    {
        public event Action<IEvolveScreen> ScreenOpened;
        public event Action<IEvolveScreen> InfoChanged;
        public event Action<IEvolveScreen> RequiresAttention;
        public event Action<IEvolveScreen> ScreenClosed;
        public event Action<IEvolveScreen, bool> ActiveChanged;
        public event Action<IEvolveScreen> ScreenDisposed;

        private bool _isReviewed;

        [ShowInInspector, ReadOnly]
        public bool IsReviewed
        {
            get =>  _isReviewed && _screenStateAggregator.IsReviewed;
            private set => _isReviewed = value;
        }

        [ShowInInspector, ReadOnly]
        public bool NeedAttention => IsNextAgeAffordable() || _screenStateAggregator.NeedAttention;

        public string Info => "Evolution";
        
        public event Action StateChanged;
        public event Action ButtonStateChanged;
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IMiniShopProvider _miniShopProvider;
        private readonly ITimelineInfoScreenProvider _timelineInfoProvider;
        private readonly IAudioService _audioService;
        private readonly IUINotifier _uiNotifier;
        private readonly IMyLogger _logger;
        private readonly IConfigRepository _config;
        
        private ITimelineConfigRepository TimelineConfigRepository => _config.TimelineConfigRepository;
        private IIconConfigRepository IconConfig => _config.IconConfigRepository;

        private readonly AgeIconContainer _ageIconContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private CurrencyCell Cell { get; set; }


        [ShowInInspector, ReadOnly]
        private readonly ScreenStateAggregator _screenStateAggregator;

        private readonly CurrencyBank _bank;


        public EvolveScreenPresenter(
            IUserContainer userContainer,
            IConfigRepository configRepository,
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            IMiniShopProvider miniShopProvider,
            ITimelineInfoScreenProvider timelineInfoProvider,
            IAudioService audioService,
            IUINotifier uiNotifier,
            AgeIconContainer ageIconContainer,
            CurrencyBank bank)
        {
            _bank = bank;
            _userContainer = userContainer;
            _config = configRepository;
            _logger = logger;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _miniShopProvider = miniShopProvider;
            _timelineInfoProvider = timelineInfoProvider;
            _audioService = audioService;
            _uiNotifier = uiNotifier;
            _ageIconContainer = ageIconContainer;
            gameInitializer.OnPostInitialization += Init;
            
            _screenStateAggregator = new ScreenStateAggregator();

            _uiNotifier.RegisterScreen(this, this);
        }

        private async void Init()
        {
            _ageNavigator.AgeChanged += OnAgeChanged;
            Cell = _bank.GetCell(CurrencyType.Coins);
            Cell.OnStateChanged += OnCurrenciesStateChanged;
            Cell.OnAmountAdded += OnCurrenciesAdded;

            IsReviewed = !NeedAttention;

            await UniTask.WaitForSeconds(2);
            OnCurrenciesAdded(0);
        }

        void IDisposable.Dispose()
        {
            _uiNotifier.UnregisterScreen(this);
            TimelineState.NextAgeOpened -= OnAgeChanged;
            _gameInitializer.OnPostInitialization -= Init;
            Cell.OnStateChanged -= OnCurrenciesStateChanged;
            Cell.OnAmountAdded -= OnCurrenciesAdded;
        }

        void IEvolveScreenPresenter.OnScreenOpen()
        {
            IsReviewed = true;
            ScreenOpened?.Invoke(this);
        }

        void IEvolveScreenPresenter.OnScreenClosed()
        {
            ScreenClosed?.Invoke(this);
        }
        
        void IEvolveScreenPresenter.OnScreenDisposed()
        {
            _logger.Log("EVOLVE SCREEN DISPOSED", DebugStatus.Info);
            ScreenDisposed?.Invoke(this);
        }
        
        void IEvolveScreenPresenter.OnScreenActiveChanged(bool isActive)
        {
            ActiveChanged?.Invoke(this, isActive);
        }

        public Sprite GetCurrencyIcon() => 
            IconConfig.GetCurrencyIconFor(CurrencyType.Coins);

        private void OnCurrenciesAdded(double _)
        {
            if (NeedAttention)
            {
                RequiresAttention?.Invoke(this);
            }
            _logger.Log("EVOLVE SCREEN NEED ATTENTION", DebugStatus.Warning);
        }
        
        private void OnCurrenciesStateChanged() => ButtonStateChanged?.Invoke();

        private void OnAgeChanged() => StateChanged?.Invoke();


        public bool IsNextAgeAffordable()
        {
            if(TimelineState.AgeId == TimelineConfigRepository.LastAgeIdx()) return false;
            
            if (TimelineState.MaxBattle > TimelineState.AgeId)
                return true;

            return Cell.IsEnough(TimelineConfigRepository.GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId).Price);
        }

        public float GetEvolutionPrice()
        {
            if (TimelineState.MaxBattle > TimelineState.AgeId) return -1;
            return TimelineConfigRepository.GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId).Price;
        }

        public string GetTimelineNumber() =>
            $"Timeline {TimelineState.TimelineId + 1}";

        public Sprite GetCurrentAgeIcon()
        {
            var currentAgeConfig = TimelineConfigRepository.GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId);
            
            var iconReference =currentAgeConfig .GetIconReference();
            Sprite icon = _ageIconContainer.Get(iconReference.Atlas.AssetGUID).Get(iconReference.IconName);
            
            return icon;
        }

        public string GetCurrentAgeName()
        {
            var currentAgeConfig = TimelineConfigRepository.GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId);
            return currentAgeConfig .Name;
        }

        public Sprite GetNextAgeIcon()
        {
            var nextAgeConfig = TimelineConfigRepository.GetRelatedAge(
                TimelineState.TimelineId, 
                Mathf.Min(TimelineState.AgeId + 1, TimelineConfigRepository.LastAgeIdx()));

            var iconReference = nextAgeConfig.GetIconReference();
            Sprite icon = _ageIconContainer.Get(iconReference.Atlas.AssetGUID).Get(iconReference.IconName);
            
            return icon;
        }

        public string GetNextAgeName()
        {
            var nextAgeConfig = TimelineConfigRepository.GetRelatedAge(
                TimelineState.TimelineId, 
                Mathf.Min(TimelineState.AgeId + 1, TimelineConfigRepository.LastAgeIdx()));
            return nextAgeConfig .Name;
        }

        public async void OnEvolveClicked()
        {
            _audioService.PlayButtonSound();
            var screen = await _timelineInfoProvider.Load();
            //var isExited = await screen.Value.ShowScreenWithTransitionAnimation();
            //if (isExited) _timelineInfoProvider.Dispose();
        }

        public async void OnInactiveEvolveClicked()
        {
            if (!_miniShopProvider.IsUnlocked) return;
            var popup = await _miniShopProvider.Load();
            bool isExit = await popup.Value.ShowAndAwaitForDecision(GetEvolutionPrice());
            if (isExit) _miniShopProvider.Dispose();
        }

        public bool CanEvolve() =>
            IsNextAgeAffordable() || TimelineState.MaxBattle > TimelineState.AgeId;

        public async void OnInfoClicked()
        {
            _audioService.PlayButtonSound();
            var screen = await _timelineInfoProvider.Load();
            //var isExited = await screen.Value.ShowScreen();
           // if (isExited) _timelineInfoProvider.Dispose();
        }
        
        //Debug
        [Button]
        public void RequiredAttention()
        {
            OnCurrenciesAdded(0);
        }

        void IGameScreenListener<ITravelScreen>.OnScreenOpened(ITravelScreen screen) => UpdateState(screen);
        void IGameScreenListener<ITravelScreen>.OnInfoChanged(ITravelScreen screen) { }
        void IGameScreenListener<ITravelScreen>.OnRequiresAttention(ITravelScreen screen) => UpdateState(screen);
        void IGameScreenListener<ITravelScreen>.OnScreenClosed(ITravelScreen screen) { }
        void IGameScreenListener<ITravelScreen>.OnScreenActiveChanged(ITravelScreen screen, bool isActive) { }
        void IGameScreenListener<ITravelScreen>.OnScreenDisposed(ITravelScreen screen) { }

        private void UpdateState(IGameScreen screen)
        {
            if (_screenStateAggregator.UpdateState(screen))
            {
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
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