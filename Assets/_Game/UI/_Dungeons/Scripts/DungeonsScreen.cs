using _Game.Core.Services._Camera;
using UnityEngine;

namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonsScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private DungeonListView _dungeonListView;
        public DungeonListView DungeonListView => _dungeonListView;

        private IDungeonsScreenPresenter _presenter;
        
        public void Construct(
            IWorldCameraService cameraService,
            IDungeonsScreenPresenter presenter)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = presenter;
            _canvas.enabled = false;
            _presenter.Screen = this;
        }

        public void Show()
        {
            _canvas.enabled = true;
            _presenter.OnScreenOpened();
        }
        
        
        public void Hide()
        {
            _canvas.enabled = false;
            _presenter.OnScreenClosed();
        }

        public void Dispose() => 
            _presenter.OnScreeDispose();

        public void SetActive(bool isActive)
        {
            _presenter.OnScreeActiveChanged(isActive);
            gameObject.SetActive(isActive);
        }
    }
}
