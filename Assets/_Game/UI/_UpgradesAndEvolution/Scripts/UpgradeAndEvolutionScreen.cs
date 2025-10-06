using _Game.Core.Services._Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionScreen : MonoBehaviour
    {
        public ToggleButton UpgradesButton => _upgradesButton;
        public ToggleButton EvolutionButton => _evolutionButton;
        public TutorialStep EvolutionStep => _evolutionStep;

        public QuickBoostInfoPanel QuickBoostInfoPanel => _quickBoostInfoPanel;


        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private ToggleButton _upgradesButton;
        [SerializeField, Required] private ToggleButton _evolutionButton;

        [SerializeField, Required] private QuickBoostInfoPanel _quickBoostInfoPanel;
        
        [SerializeField, Required] private TutorialStep _evolutionStep;

        private IUpgradesAndEvolutionScreenPresenter _presenter;

        public void Construct(
            IWorldCameraService cameraService,
            IUpgradesAndEvolutionScreenPresenter presenter)
        {
            _presenter = presenter;
            _presenter.Screen = this;
            
            _canvas.worldCamera = cameraService.UICameraOverlay;

            _canvas.enabled = false;
        }

        public void Show()
        {
            _presenter.OnUpgradesAndEvolutionScreenOpened();
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _presenter.OnUpgradesAndEvolutionScreenClosed();
        }

        public void Dispose() => 
            _presenter.OnScreeDispose();

        public void SetActive(bool isActive)
        {
            _presenter.OnScreenActiveChanged(isActive);
            gameObject.SetActive(isActive);
        }
    }
}
