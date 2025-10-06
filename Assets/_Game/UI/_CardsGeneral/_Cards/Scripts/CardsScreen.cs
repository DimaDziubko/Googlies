using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._CardsGeneral._Summoning.Scripts;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreen : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private CardsContainer _container;

        [SerializeField, Required] private SummoningView _summoningView;
        [SerializeField, Required] private TransactionButton _x1CardBtn;
        [SerializeField, Required] private TransactionButton _x10CardBtn;

        [SerializeField, Required] private TutorialStep _cardsStep;
        
        public TutorialStep CardsStep => _cardsStep;
        public CardsContainer Container => _container;
        public TransactionButton X1CardBtn => _x1CardBtn;
        public TransactionButton X10CardBtn => _x10CardBtn;
        public SummoningView SummoningView => _summoningView;

        private ICardsScreenPresenter _presenter;

        public void Construct(
            IWorldCameraService cameraService,
            ICardsScreenPresenter cardsScreenPresenter,
            IMyLogger logger)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = cardsScreenPresenter;
            _presenter.Screen = this;
            _canvas.enabled = false;
        }


        public void Show()
        {
            _presenter.OnCardScreenOpened();
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            Cleanup();
            _presenter.OnCardScreenClosed();
        }

        private void Cleanup()
        {
            _x1CardBtn.Cleanup();
            _x10CardBtn.Cleanup();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnCardScreenActiveChanged(isActive);
        }
    }
}