using System.Collections.Generic;
using System.Linq;
using _Game.Core._DataPresenters.UnitBuilderDataPresenter;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._GameplayUI.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderViewController : 
        IUnitBuilder,
        IStartGameListener,
        IPauseListener,
        IStopGameListener
    {
        private readonly GameplayUI _gameplayUI;
        private readonly IUnitSpawnProxy _unitSpawnProxy;
        
        private readonly IAudioService _audioService;
        private readonly ITutorialManager _tutorialManager;
        private readonly IUnitBuilderModel _model;
        private readonly IFoodContainer _foodContainer;
        private readonly IMyLogger _logger;

        private UnitBuilderUI UnitBuilderUI => _gameplayUI.UnitBuilderUI;
        private TutorialStep TutorialStep => UnitBuilderUI.TutorialStep;

        private readonly List<UnitBuildBtnController> _controllers = new(3);

        public UnitBuilderViewController(
            GameplayUI gameplayUI,
            IUnitSpawnProxy unitSpawnProxy,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            IUnitBuilderModel model,
            IFoodContainer foodContainer,
            IMyLogger logger)
        {
            _gameplayUI = gameplayUI;
            _unitSpawnProxy = unitSpawnProxy;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _logger = logger;
            _model = model;
            _foodContainer = foodContainer;
        }

        void IStartGameListener.OnStartBattle()
        {
            StartBuilder();
        }

        private void StartBuilder()
        {
            _foodContainer.OnStateChanged += OnFoodAmountChanged;
            
            InitButtons();
            
            _gameplayUI.Show();
            
            _tutorialManager.Register(TutorialStep);
        }

        private void InitButtons()
        {
            foreach (var btnModel in _model.GetBtnModels())
            {
                var view = UnitBuilderUI
                    .Buttons
                    .FirstOrDefault(x => x.Type == btnModel.Type);
                var controller = new UnitBuildBtnController(btnModel, view, this, _audioService, _logger);
                controller.Initialize();
                _controllers.Add(controller);
            }
            
            OnFoodAmountChanged();
        }
        
        private void OnFoodAmountChanged()
        {
            foreach (var controller in _controllers)
            {
                controller.OnFoodBalanceChanged(_foodContainer.Amount);
                if (controller.IsButtonReady)
                {
                    TutorialStep.ShowStep();
                }
            }
        }
        
        void IUnitBuilder.Build(UnitType type, int foodPrice)
        {
            if(!_foodContainer.IsEnough(foodPrice)) return;

            TutorialStep.CompleteStep();
            
            _unitSpawnProxy.SpawnPlayerUnit(type, Skin.Ally).Forget();
            _foodContainer.Spend(foodPrice);
        }
        
        void IPauseListener.SetPaused(bool isPaused)
        {
            foreach (var presenter in _controllers)
            {
                presenter.SetPaused(isPaused);
            }
        }

        void IStopGameListener.OnStopBattle()
        {
            _gameplayUI.Hide();

            foreach (var controller in _controllers)
            {
                controller.Dispose();
                controller.HideBtn();
            }
            
            _controllers.Clear();

            TutorialStep.CancelStep();
            _tutorialManager.UnRegister(TutorialStep);

            _foodContainer.OnStateChanged -= OnFoodAmountChanged;
        }
    }
}