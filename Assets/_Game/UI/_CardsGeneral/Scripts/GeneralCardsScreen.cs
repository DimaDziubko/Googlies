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
        public SimpleToggleButton CardBtn => _cardsBtn;
        public SimpleToggleButton SkillsBtn => _skillsBtn;
        public SimpleToggleButton RunesBtn => _runesBtn;
        public SimpleToggleButton HeroesBtn => _heroesBtn;
        public TutorialStep SkillsStep => _skillsStep;

        [SerializeField, Required] private Canvas _canvas;
        
        [SerializeField, Required] private SimpleToggleButton _cardsBtn;
        [SerializeField, Required] private SimpleToggleButton _skillsBtn;
        [SerializeField, Required] private SimpleToggleButton _runesBtn;
        [SerializeField, Required] private SimpleToggleButton _heroesBtn;

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