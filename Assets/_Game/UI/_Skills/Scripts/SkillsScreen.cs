using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.UI._Currencies;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SkillsScreen : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private SkillsContainer _container;
        
        [SerializeField, Required] private TransactionButton _x1SkillBtn;
        [SerializeField, Required] private TransactionButton _x10SkillBtn;
        
        [SerializeField, Required] private CurrencyView _skillsPetFeedView;
        [SerializeField, Required] private SkillSlotListView _skillSlotListView;
        
        public SkillSlotListView SkillSlotListView => _skillSlotListView;
        public CurrencyView SkillsPetFeedView => _skillsPetFeedView;
        public SkillsContainer Container => _container;
        public TransactionButton X1SkillBtn => _x1SkillBtn;
        public TransactionButton X10SkillBtn => _x10SkillBtn;

        private ISkillsScreenPresenter _presenter;

        public void Construct(
            IWorldCameraService cameraService,
            ISkillsScreenPresenter skillsScreenPresenter,
            IMyLogger logger)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = skillsScreenPresenter;
            _presenter.Screen = this;
            _canvas.enabled = false;
        }
        
        public void Show()
        {
            _presenter.OnSkillScreenOpened();
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _presenter.OnSkillScreenClosed();
        }

        public void Dispose()
        {
            Cleanup();
            _presenter.OnSkillScreenDisposed();
        }

        private void Cleanup()
        {
            _x1SkillBtn.Cleanup();
            _x10SkillBtn.Cleanup();
        }

        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}