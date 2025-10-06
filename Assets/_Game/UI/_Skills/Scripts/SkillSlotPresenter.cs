using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Utils.Disposable;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlotPresenter
    {
        private readonly SkillSlot _slot;
        private readonly SkillSlotView _view;
        
        private readonly IAudioService _audioService;

        private readonly IWorldCameraService _cameraService;
        private readonly IIconConfigRepository _config;
        private readonly BoostContainer _boosts;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly SkillSlotContainer _container;

        private SkillPopupPresenter _activeSkillPopupPresenter;
        private SkillPopupProvider _activeSkillPopupProvider;

        public SkillSlotPresenter(
            SkillSlot slot, 
            SkillSlotView view,
            IAudioService audioService,
            IWorldCameraService cameraService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IUserContainer userContainer,
            SkillSlotContainer container,
            IMyLogger logger)
        {
            _slot = slot;
            _view = view;
            _audioService = audioService;
            _cameraService = cameraService;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            _userContainer = userContainer;
            _container = container;
        }

        public void Initialize()
        {
            _slot.Equipped += OnSlotEquippedChanged;
            _slot.UnEquipped += OnSlotEquippedChanged;
            
            _view.SlotClicked += OnClicked;

            _view.SetLocked(_slot.IsLocked);
            _view.SetLockedInfo($"Timeline {_slot.TimelineThreshold}");

            OnSlotEquippedChanged();
        }

        public void Dispose()
        {
            _slot.Equipped -= OnSlotEquippedChanged;
            _slot.UnEquipped -= OnSlotEquippedChanged;

            _view.SlotClicked -= OnClicked;
        }

        private void OnSlotEquippedChanged()
        {
            _logger.Log("SKILL SLOT PRESENTER ON EQUIP CHANGED", DebugStatus.Info);
            
            if (_slot.IsEquipped && _slot.SkillModel != null)
            {
                _view.SetIconActive(true);
                _view.SetIcon(_slot.SkillModel.GetIcon());
            }
            else
            {
                _view.SetIconActive(false);
            }
        }
        

        private void OnClicked()
        {
            PlayButtonSound();

            if (_slot.IsEquipped && _slot.SkillModel != null)
            {
                ShowSkillPopup(_slot.SkillModel);
            }
            
            _logger.Log("SKILL SLOT PRESENTER CLICKED", DebugStatus.Info);
        }
        
        private async void ShowSkillPopup(SkillModel skillModel)
        {
            skillModel.SetNew(false);

            if (_activeSkillPopupPresenter == null)
            {
                _activeSkillPopupPresenter = new SkillPopupPresenter(skillModel, _container, _logger);
            }
            else
            {
                _activeSkillPopupPresenter.SetModel(skillModel);
            }

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
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}