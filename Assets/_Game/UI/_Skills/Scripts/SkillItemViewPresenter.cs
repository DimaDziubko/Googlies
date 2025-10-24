using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils.Disposable;

namespace _Game.UI._Skills.Scripts
{
    public class SkillItemViewPresenter
    {
        public SkillItemView View => _view;
        public SkillModel Model => _skillModel;

        private SkillModel _skillModel;
        private SkillItemView _view;

        private readonly SkillViewPresenter _viewPresenter;

        private ISkillPopupProvider _activeSkillPopupProvider;
        private IWorldCameraService _cameraService;
        private IAudioService _audioService;
        private IIconConfigRepository _config;
        private BoostContainer _boosts;
        private SkillPopupPresenter _activeSkillPopupPresenter;
        private IMyLogger _logger;
        private IUserContainer _userContainer;
        private SkillSlotContainer _skillSlotContainer;

        public SkillItemViewPresenter(
            SkillModel skillModel,
            SkillItemView view, 
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _logger = logger;
            _skillModel = skillModel;
            _view = view;
            _viewPresenter = new SkillViewPresenter(skillModel, view.SkillView, userContainer);
        }

        public SkillItemViewPresenter(SkillModel skillModel,
            SkillItemView view,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IMyLogger logger,
            IUserContainer userContainer,
            SkillSlotContainer skillSlotContainer)
        {
            _skillModel = skillModel;
            _view = view;
            _cameraService = cameraService;
            _audioService = audioService;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            _skillSlotContainer = skillSlotContainer;
            
            _userContainer = userContainer;
            _viewPresenter = new SkillViewPresenter(skillModel, view.SkillView, userContainer);
        }

        public void InitializeNotClickable()
        {
            _viewPresenter.Initialize();

            _skillModel.Skill.CountChanged += OnNotClickableStateChanged;
            _skillModel.Skill.OnLevelUp += OnLevelChanged;
            _skillModel.NewMarkChanged += OnNewMarkChanged;
            _skillModel.Skill.OnAscensionLevelUp += OnOnAscensionLevelUp;
            
            OnNotClickableStateChanged();
            
            _view.SetEquipped(false);
            
            _logger.Log("INITIALIZE NOT CLICKABLE", DebugStatus.Info);
        }

        public void Initialize()
        {
            _viewPresenter.Initialize();

            _skillModel.Skill.CountChanged += OnStateChanged;
            _skillModel.Skill.OnLevelUp += OnLevelChanged;
            _view.ActiveSkillClicked += OnSkillClicked;
            _skillModel.Skill.SkillEquippedChanged += OnSkillEquippedChanged;
            //_skillModel.NewMarkChanged += OnNewMarkChanged;
            _skillModel.Skill.OnAscensionLevelUp += OnOnAscensionLevelUp;

            OnStateChanged();
            OnSkillEquippedChanged(_skillModel.Skill);
            
            _logger.Log("INITIALIZE CLICKABLE", DebugStatus.Info);
        }

        public void Dispose()
        {
            _viewPresenter.Dispose();
            _skillModel.Skill.CountChanged -= OnNotClickableStateChanged;
            _skillModel.Skill.CountChanged -= OnStateChanged;
            _skillModel.Skill.OnLevelUp -= OnLevelChanged;
            _view.ActiveSkillClicked -= OnSkillClicked;
            _skillModel.Skill.SkillEquippedChanged -= OnSkillEquippedChanged;
            _skillModel.NewMarkChanged -= OnNewMarkChanged;
            _skillModel.Skill.OnAscensionLevelUp -= OnOnAscensionLevelUp;
            
            _logger.Log("DISPOSE SKILL ITEM VIEW", DebugStatus.Info);
        }

        private void OnOnAscensionLevelUp(ActiveSkill skill) => SetAscensionLevel();
        private void OnNewMarkChanged() => _view.SkillView.SetNewNotifierActive(_skillModel.IsNew);

        private void OnSkillEquippedChanged(ActiveSkill skill)
        {
            _view.SetEquipped(skill.Equipped);
            _logger.Log("SKILL ITEM PRESENTER ON EQUIP CHANGED", DebugStatus.Info);
        }
        
        private void OnLevelChanged(ActiveSkill skill) => OnStateChanged();

        private void OnNotClickableStateChanged()
        {
            SetProgress();
            SetLevel();
            SetReadyForUpgrade();
            SetAscensionLevel();
        }

        private void OnStateChanged()
        {
            SetProgress();
            SetLevel();
            SetReadyForUpgrade();
            SetAscensionLevel();
            OnNewMarkChanged();
        }

        private void SetAscensionLevel() => 
            _view.EnableStars(_skillModel.Skill.AscensionLevel);

        private void SetReadyForUpgrade() => 
            _view.SetActiveUpgradeNotifier(_skillModel.IsReadyForUpgrade);

        private void SetLevel() => 
            _view.SetLevel($"Lv {_skillModel.Skill.Level}");

        private void SetProgress()
        {
            if (_skillModel.IsMaxLevel)
            {
                _view.SetProgress("max.");
                _view.SetProgressValue(1);
            }
            else
            {
                _view.SetProgress($"{_skillModel.Skill.Count}/{_skillModel.GetUpgradeCount()}");
                _view.SetProgressValue(_skillModel.ProgressValue);
            }

            _view.SetBarColor(_skillModel.GetBarColorName());
        }

        private async void OnSkillClicked()
        {
            _audioService.PlayButtonSound();

            _skillModel.SetNew(false);

            _activeSkillPopupPresenter ??= new SkillPopupPresenter(Model, _skillSlotContainer, _logger);

            _activeSkillPopupProvider ??= new SkillPopupProvider(
                _activeSkillPopupPresenter,
                _cameraService,
                _audioService,
                _config,
                _boosts,
                _userContainer,
                _logger);

            Disposable<SkillPopup> popup = await _activeSkillPopupProvider.Load();
            bool isConfirmed = await popup.Value.ShowDetailsAndAwaitForExit();
            if (isConfirmed) popup.Dispose();
        }
    }
}