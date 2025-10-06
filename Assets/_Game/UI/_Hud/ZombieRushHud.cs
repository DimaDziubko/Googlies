using _Game.Core._Logger;
using _Game.Core.Services._Camera;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class ZombieRushHud : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ToggleWithSpriteSwap _pauseView;
        [SerializeField] private TMP_Text _ratsCounter;
        public ToggleWithSpriteSwap PauseView => _pauseView;

        public void Construct(
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
        }

        public void UpdateRatsCounter(string value) => _ratsCounter.text = value;
        
        public void Show() => _canvas.enabled = true;

        public void Hide() => _canvas.enabled = false;

    }
}