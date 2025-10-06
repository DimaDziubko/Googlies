using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlotListViewPresenter
    {
        private readonly SkillSlotContainer _container;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;
        private readonly IIconConfigRepository _config;
        private readonly BoostContainer _boosts;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private SkillSlotListView _view;

        [ShowInInspector, ReadOnly]
        private readonly List<SkillSlotPresenter> _presenters = new();

        public SkillSlotListViewPresenter(
            SkillSlotContainer container, 
            SkillSlotListView view,
            IAudioService audioService,
            IWorldCameraService cameraService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _container = container;
            _view = view;
            _audioService = audioService;
            _audioService = audioService;
            _cameraService = cameraService;
            _config = config;
            _boosts = boosts;
            _logger = logger;
            _userContainer = userContainer;
        }

        public void Initialize()
        {
            foreach (var slot in _container.GetSlots())
            {
                var view = _view.SkillSlotView.FirstOrDefault(x => x.Id == slot.Id);
                
                if (view != null)
                {
                    SkillSlotPresenter presenter = new SkillSlotPresenter(
                        slot, 
                        view, 
                        _audioService, 
                        _cameraService, 
                        _config, 
                        _boosts, 
                        _userContainer,
                        _container,
                        _logger);
                    
                    _presenters.Add(presenter);
                    presenter.Initialize();
                }
            }
            
            _logger.Log("SKILL SLOT LIST VIEW PRESENTER INITIALIZED", DebugStatus.Info);
        }

        public void SetView(SkillSlotListView view)
        {
            _view = view;
        }
        
        public void Dispose()
        {
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }
            
            _presenters.Clear();
            
            _logger.Log("SKILL SLOT LIST VIEW PRESENTER DISPOSED", DebugStatus.Info);
        }
    }
}