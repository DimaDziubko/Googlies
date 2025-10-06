using System;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.Utils.Extensions;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskPresenter
    {
        public event Action CompleteClicked;

        private readonly DailyTaskModel _model;
        private readonly DailyTaskView _view;

        private readonly IIconConfigRepository _config;
        private readonly CurrencyBank _bank;
        private readonly IAudioService _audioService;
        private readonly IUserContainer _userContainer;
        private readonly ITutorialManager _tutorialManager;
        private readonly StartBattleScreenEventsRouter _router;
        private readonly IMyLogger _logger;

        public DailyTaskPresenter(
            DailyTaskModel model,
            DailyTaskView view,
            IConfigRepository config,
            IUserContainer userContainer,
            CurrencyBank bank,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            StartBattleScreenEventsRouter router,
            IMyLogger logger)
        {
            _router = router;
            _userContainer = userContainer;
            _model = model;
            _view = view;
            _config = config.IconConfigRepository;
            _bank = bank;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _logger = logger;
        }

        public void Initialize()
        {
            _tutorialManager.Register(_view.DailyTaskStep);
            _router.StartBattleScreenOpened += OnBattleScreenOpened;
            _router.StartBattleScreenClosed += OnBattleScreenClosed;

            if (!_view.gameObject.activeSelf) _view.gameObject.SetActive(true);
            _view.SetDailyInfo(_model.DailyInfo);
            _view.RewardView.SetAmount(_model.Reward.FirstOrDefault().Amount.ToCompactFormat());
            _view.RewardView.SetIcon(_config.GetCurrencyIconFor(_model.Reward.FirstOrDefault().Type));

            _model.State.ProgressChanged += OnProgressChanged;
            _model.DescriptionChanged += OnProgressChanged;
            _model.TargetChanged += OnProgressChanged;

            _view.OnButtonClick += OnCompletedClicked;

            _view.Show();

            OnProgressChanged();
        }

        private void OnBattleScreenClosed()
        {
            _view.DailyTaskStep.CancelStep();
            _tutorialManager.UnRegister(_view.DailyTaskStep);
            _logger.Log("OnBattleScreenClosed", DebugStatus.Info);
        }

        private void OnBattleScreenOpened()
        {
            _logger.Log("OnBattleScreenOpened", DebugStatus.Info);
            _tutorialManager.Register(_view.DailyTaskStep);
            HandleTutorialStepAndNotification();
        }

        private void HandleTutorialStepAndNotification()
        {
            if (_model.IsReady)
            {
                _view.PlayNotification();
                _view.DailyTaskStep.ShowStep(0.5f);
            } 
            else
            {
                _view.DailyTaskStep.CancelStep();
                _view.StopNotification();
            }
        }

        private void OnProgressChanged()
        {
            _view.SetProgress(_model.Progress);
            _view.SetInteractable(_model.IsReady);

            HandleTutorialStepAndNotification();
        }

        public void Dispose()
        {
            _view.DailyTaskStep.CancelStep();
            _tutorialManager.UnRegister(_view.DailyTaskStep);

            _router.StartBattleScreenOpened -= OnBattleScreenOpened;
            _router.StartBattleScreenClosed -= OnBattleScreenClosed;

            _model.DescriptionChanged -= OnProgressChanged;
            _model.State.ProgressChanged -= OnProgressChanged;
            _model.TargetChanged -= OnProgressChanged;
            _view.OnButtonClick -= OnCompletedClicked;
        }

        private void OnCompletedClicked()
        {
            _view.DailyTaskStep.CompleteStep();
            _audioService.PlayButtonSound();
            _view.PlayHide(OnAnimationCompleted);
            _view.StopNotification();
        }

        private void OnAnimationCompleted()
        {
            _bank.Add(_model.Reward);
            _userContainer.DailyTaskStateHandler.AddCompleteDailyTask();
            CompleteClicked?.Invoke();
        }

        public sealed class Factory : PlaceholderFactory<DailyTaskModel, DailyTaskView, DailyTaskPresenter>
        {

        }
    }
}