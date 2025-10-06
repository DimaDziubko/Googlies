using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.UI.Global;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillsScreenPresenter :
        ISkillsScreenPresenter,
        IDisposable,
        IGameScreenEvents<ISkillsScreen>,
        IGameScreenListener<IMenuScreen>,
        ISkillsScreen
    {
        public event Action<ISkillsScreen> ScreenOpened;
        public event Action<ISkillsScreen> InfoChanged;
        public event Action<ISkillsScreen> RequiresAttention;
        public event Action<ISkillsScreen> ScreenClosed;
        public event Action<ISkillsScreen, bool> ActiveChanged;
        public event Action<ISkillsScreen> ScreenDisposed;

        [ShowInInspector, ReadOnly] public bool IsReviewed { get; private set; }

        [ShowInInspector, ReadOnly]
        // public bool NeedAttention => _productBuyer.CanBuy(_x1SkillBundle) ||
        //                              _productBuyer.CanBuy(_x10SkillBundle) ||
        //                              IsAnyUpgradeAvailable();
        
        public bool NeedAttention => _productBuyer.CanBuy(_x10SkillBundle) || _needAttentionOnUnlock;
        private bool IsAnyUpgradeAvailable() => _skillService.IsAnyUpgradeAvailable;
        
        public string Info => _skillService.SkillProgressInfo;

        private readonly ISkillConfigRepository _skillConfigRepository;
        private readonly IGameInitializer _gameInitializer;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly IUIFactory _uiFactory;
        private readonly IIconConfigRepository _iconConfig;
        private readonly ISkillService _skillService;
        private readonly CurrencyBank _bank;
        private readonly BoostContainer _boosts;
        private readonly IUserContainer _userContainer;
        private readonly IUINotifier _uiNotifier;
        private readonly IParticleAttractorRegistry _register;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private SkillAppearanceScreenProvider _appearanceScreenProvider;

        public SkillsScreen Screen { get; set; }

        private Dictionary<ActiveSkill, SkillItemViewPresenter> _presenters = new();

        private CurrencyCellPresenter _skillPotionPresenter;


        [ShowInInspector, ReadOnly]
        private SkillSlotListViewPresenter _skillSlotListViewPresenter;

        private CurrencyCell Cell { get; set; }

        private ProductBuyer _productBuyer;

        private SkillBundle _x1SkillBundle;
        private SkillBundle _x10SkillBundle;
        
        private bool _needAttentionOnUnlock;

        public SkillsScreenPresenter(
            ISkillService skillService,
            IConfigRepository configRepository,
            IGameInitializer gameInitializer,
            CurrencyBank bank,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IUIFactory uiFactory,
            BoostContainer boosts,
            IUserContainer userContainer,
            IUINotifier uiNotifier,
            IMyLogger logger,
            IParticleAttractorRegistry register,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _featureUnlockSystem = featureUnlockSystem;
            _skillService = skillService;
            _skillConfigRepository = configRepository.SkillConfigRepository;
            _iconConfig = configRepository.IconConfigRepository;
            _gameInitializer = gameInitializer;
            _bank = bank;
            _logger = logger;
            _register = register;
            _cameraService = cameraService;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _boosts = boosts;
            _userContainer = userContainer;
            _uiNotifier = uiNotifier;
            
            _gameInitializer.OnPostInitialization += Init;
            
            _uiNotifier.RegisterScreen(this, this);
        }

        private void OnFetureUnlocked(Feature feature)
        {
            if (feature == Feature.Skills)
            {
                _needAttentionOnUnlock = true; 
                RequiresAttention?.Invoke(this);
            }
        }

        private void Init()
        {
            _appearanceScreenProvider = new SkillAppearanceScreenProvider(_cameraService, _audioService, _userContainer);

            Cell = _bank.GetCell(CurrencyType.SkillPotion);

            _productBuyer = new ProductBuyer(_bank, _logger);

            _x1SkillBundle = new SkillBundle(1, new[]
            {
                new CurrencyData()
                {
                    Type = CurrencyType.SkillPotion,
                    Amount = _skillService.GetX1SkillPrice(),
                    Source = ItemSource.Skills
                },
            });

            _x10SkillBundle = new SkillBundle(10, new[]
            {
                new CurrencyData()
                {
                    Type = CurrencyType.SkillPotion,
                    Amount = _skillService.GetX10SkillPrice(),
                    Source = ItemSource.Skills
                },
            });
            
            Subscribe();

            IsReviewed = !NeedAttention;
            OnStateChanged(0);
        }

        private void Subscribe()
        {
            _productBuyer.ProductBought += _skillService.SkillBundleBought;
            _skillService.SkillsAdded += OnSkillsAdded;

            Cell.OnStateChanged += OnCurrencyStateChanged;
            Cell.OnAmountAdded += OnStateChanged;
            
            _featureUnlockSystem.FeatureUnlocked += OnFetureUnlocked;
        }

        private void OnStateChanged(double _)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                CheckButtonsState();
                RequiresAttention?.Invoke(this);
            }
        }

        private void OnCurrencyStateChanged() => CheckButtonsState();

        private void CheckButtonsState()
        {
            if (Screen != null)
            {
                Screen.X1SkillBtn.SetActive(_skillService.Generator.TotalRemainingSkills >= _x1SkillBundle.Quantity);
                bool isX1CardActive = _productBuyer.CanBuy(_x1SkillBundle);
                Screen.X1SkillBtn.SetInteractable(isX1CardActive);

                Screen.X10SkillBtn.SetActive(_skillService.Generator.TotalRemainingSkills >= _x10SkillBundle.Quantity);
                bool isX10CardActive = _productBuyer.CanBuy(_x10SkillBundle);
                Screen.X10SkillBtn.SetInteractable(isX10CardActive);
            }
        }

        void ISkillsScreenPresenter.OnSkillScreenOpened()
        {
            if (Screen != null)
            {
                InitButtons();
                InitCurrency();

                InitSkills();
                InitEquipSells();

                IsReviewed = true;
                _needAttentionOnUnlock = false;
                ScreenOpened?.Invoke(this);
            }
        }

        private void InitEquipSells()
        {
            if (_skillSlotListViewPresenter != null)
            {
                _skillSlotListViewPresenter.SetView(Screen.SkillSlotListView);
            }
            else
            {
                _skillSlotListViewPresenter ??= new SkillSlotListViewPresenter(
                    _skillService.SkillSlotContainer,
                    Screen.SkillSlotListView,
                    _audioService,
                    _cameraService,
                    _iconConfig,
                    _boosts,
                    _userContainer,
                    _logger);
            }

            _skillSlotListViewPresenter.Initialize();
        }

        private void InitCurrency()
        {
            if (_skillPotionPresenter != null)
            {
                _skillPotionPresenter.SetData(Cell, Screen.SkillsPetFeedView);
            }
            else
            {
                _skillPotionPresenter = new CurrencyCellPresenter(Cell, Screen.SkillsPetFeedView, _iconConfig, _register, _audioService);
            }
           
            _skillPotionPresenter.Initialize();
        }

        private void InitSkills()
        {
            foreach (var skill in _skillService.Skills.Values)
            {
                CreateSkill(skill);
            }
        }

        void ISkillsScreenPresenter.OnSkillScreenClosed()
        {
            ScreenClosed?.Invoke(this);
            Cleanup();
        }

        void ISkillsScreenPresenter.OnSkillScreenDisposed()
        {
            DeepCleanup();
            _logger.Log("SKILL SCREEN DISPOSED", DebugStatus.Info);
        }

        private void InitButtons()
        {
            Screen.X1SkillBtn.SetActive(_skillService.Generator.TotalRemainingSkills >= _x1SkillBundle.Quantity);
            Screen.X10SkillBtn.SetActive(_skillService.Generator.TotalRemainingSkills >= _x10SkillBundle.Quantity);
            
            Screen.X1SkillBtn.SetCurrencyIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.SkillPotion));
            Screen.X10SkillBtn.SetCurrencyIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.SkillPotion));

            Screen.X1SkillBtn.SetPrice(_skillConfigRepository.GetX1SkillPrice().ToString());
            Screen.X10SkillBtn.SetPrice(_skillConfigRepository.GetX10SkillPrice().ToString());

            CheckButtonsState();

            Screen.X1SkillBtn.ButtonClicked += TryToBuyX1Skill;
            Screen.X10SkillBtn.ButtonClicked += TryToBuyX10Skill;
        }


        private void Cleanup()
        {
            foreach (var presenter in _presenters.Values)
            {
                presenter.Dispose();
            }

            _skillPotionPresenter.Dispose();

            if (Screen != null)
            {
                Screen.X1SkillBtn.ButtonClicked -= TryToBuyX1Skill;
                Screen.X10SkillBtn.ButtonClicked -= TryToBuyX10Skill;
            }

            _skillSlotListViewPresenter.Dispose();
        }

        private void DeepCleanup()
        {
            foreach (var presenter in _presenters.Values)
            {
                presenter.Dispose();
                presenter.View.Release();
            }

            _presenters.Clear();

            _skillPotionPresenter?.Dispose();

            if (Screen != null)
            {
                Screen.X1SkillBtn.ButtonClicked -= TryToBuyX1Skill;
                Screen.X10SkillBtn.ButtonClicked -= TryToBuyX10Skill;
            }

            _skillSlotListViewPresenter?.Dispose();
        }

        private void SortSkills()
        {
            _presenters = _presenters
                .OrderBy(x => x.Key.Id)
                .ToDictionary(x => x.Key, x => x.Value);

            int i = 0;

            foreach (var presenter in _presenters.Values)
            {
                presenter.View.Transform.SetSiblingIndex(i);
                i++;
            }
        }

        private async void OnSkillsAdded(List<SkillModel> skillModels)
        {
            CheckButtonsState();

            List<SkillModel> newSkills = new List<SkillModel>();

            var skillAppearanceScreen = await _appearanceScreenProvider.Load();

            foreach (var skillModel in skillModels)
            {
                skillModel.SetNew(!_presenters.ContainsKey(skillModel.Skill));
                newSkills.Add(skillModel);
                AddSkill(skillModel);
            }

            var isConfirmed = await skillAppearanceScreen.Value.ShowAnimationAndAwaitForExit(newSkills);
            if (isConfirmed) _appearanceScreenProvider.Dispose();

            SortSkills();

            InfoChanged?.Invoke(this);
        }

        private void CreateSkill(SkillModel skillModel)
        {
            if (!_presenters.TryGetValue(skillModel.Skill, out var presenter))
            {
                presenter = new SkillItemViewPresenter(
                    skillModel,
                    _uiFactory.GetSkillItemView(Screen.Container.Transform),
                    _cameraService,
                    _audioService,
                    _iconConfig,
                    _boosts,
                    _logger,
                    _userContainer,
                    _skillService.SkillSlotContainer);

                _presenters.Add(skillModel.Skill, presenter);
            }

            presenter.Initialize();
        }

        private void AddSkill(SkillModel skillModel)
        {
            if (!_presenters.TryGetValue(skillModel.Skill, out var presenter))
            {
                presenter = new SkillItemViewPresenter(
                    skillModel,
                    _uiFactory.GetSkillItemView(Screen.Container.Transform),
                    _cameraService,
                    _audioService,
                    _iconConfig,
                    _boosts,
                    _logger,
                    _userContainer,
                    _skillService.SkillSlotContainer);
                
                presenter.Initialize();
                
                _presenters.Add(skillModel.Skill, presenter);
                
                _logger.Log($"SKILL ADDED {skillModel.Skill}");
            }
        }
        
        private void TryToBuyX1Skill()
        {
            if (_productBuyer.Buy(_x1SkillBundle))
            {
                _logger.Log("Skill X1 successfully bought", DebugStatus.Success);
            }
        }

        private void TryToBuyX10Skill()
        {
            if (_productBuyer.Buy(_x10SkillBundle))
            {
                _logger.Log("Skill X10 successfully bought", DebugStatus.Success);
            }
        }

        void IDisposable.Dispose()
        {
            if (_skillService != null)
            {
                _skillService.SkillsAdded -= OnSkillsAdded;
                _productBuyer.ProductBought -= _skillService.SkillBundleBought;
            }
            
            if(_gameInitializer != null)
                _gameInitializer.OnPostInitialization -= Init;

            if (Cell != null)
            {
                Cell.OnAmountAdded -= OnStateChanged;
                Cell.OnStateChanged -= OnCurrencyStateChanged;
            }

            if(_uiNotifier != null)
                _uiNotifier.UnregisterScreen(this);
            
            if(_featureUnlockSystem != null)
                _featureUnlockSystem.FeatureUnlocked -= OnFetureUnlocked;
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