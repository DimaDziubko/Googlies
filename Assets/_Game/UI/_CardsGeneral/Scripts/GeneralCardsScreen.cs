using _Game.Core.Services._Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreen : MonoBehaviour            
    {
        public ToggleButton CardBtn => _cardsBtn;
        public ToggleButton SkillsBtn => _skillsBtn;
        public ToggleButton RunesBtn => _runesBtn;
        public ToggleButton HeroesBtn => _heroesBtn;
        public TutorialStep SkillsStep => _skillsStep;

        [SerializeField, Required] private Canvas _canvas;
        
        [SerializeField, Required] private ToggleButton _cardsBtn;
        [SerializeField, Required] private ToggleButton _skillsBtn;
        [SerializeField, Required] private ToggleButton _runesBtn;
        [SerializeField, Required] private ToggleButton _heroesBtn;

        [SerializeField, Required] private QuickBoostInfoPanel _quickBoostInfoPanel;

        [SerializeField, Required] private TutorialStep _skillsStep;
        
        public QuickBoostInfoPanel QuickBoostInfoPanel => _quickBoostInfoPanel;

        private IGeneralCardsScreenPresenter _presenter;

        public void Construct(
            IWorldCameraService cameraService,
            IGeneralCardsScreenPresenter presenter)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = presenter;
            _presenter.Screen = this;
            _canvas.enabled = false;
        }

        public void Show()
        {
            _presenter.OnGeneralCardsScreenOpened();
            _canvas.enabled = true;
        }
        
        public void Hide()
        {
            _canvas.enabled = false;
            _presenter.OnGeneralCardsScreenClosed();
        }

        public void SetActive(bool isActive)
        {
            _presenter.OnScreenActiveChanged(isActive);
            gameObject.SetActive(isActive);
        }

        public void Dispose() => _presenter.OnScreenDispose();
    }
}