using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Services._Camera
{
    public class WorldCameraService : IWorldCameraService
    {
        public Camera MainCamera { get; private set; }
        public Camera UICameraOverlay { get; private set; }

        public float CameraHeight => MainCamera.orthographicSize;
        public float CameraWidth => CameraHeight * MainCamera.aspect;

        public Transform MainCameraTransform { get; private set; }

        private readonly float _baseSize = 4.2f;
        private readonly float _baseAspect = 9 / 16f;

        private readonly CameraShaker _shaker;

        public WorldCameraService(Camera mainCamera, Camera uICameraOverlay)
        {
            MainCamera = mainCamera;
            UICameraOverlay = uICameraOverlay;
            DisableMainCamera();
            Init();
            _shaker = new CameraShaker(MainCameraTransform);
        }

        private void Init()
        {
            if (MainCamera != null && MainCamera.orthographic)
            {
                ResizeCamera();
                MainCameraTransform = MainCamera.transform;
            }
        }

        private void ResizeCamera()
        {
            float currentAspect = (float)Screen.width / Screen.height;
            MainCamera.orthographicSize = _baseSize * (_baseAspect / currentAspect);
        }

        public Ray ScreenPointToRay(Vector3 mousePosition) =>
            MainCamera.ScreenPointToRay(mousePosition);

        public Vector3 ScreenToWorldPoint(Vector3 point) =>
            MainCamera.ScreenToWorldPoint(point);

        public void DisableMainCamera() =>
            MainCamera.enabled = false;

        public void EnableMainCamera() =>
            MainCamera.enabled = true;

        //Util
        public void CalculateSize() => ResizeCamera();

        public void Shake(float duration, float magnitude)
        {
            _shaker.Shake(duration, magnitude);
        }

        public bool IsVisibleOnCameraX(Vector3 position)
        {
            return position.x < CameraWidth && position.x > -CameraWidth;
        }
    }

    public class CameraShaker
    {
        private readonly Transform _cameraTransform;
        private readonly Vector3 _originalPosition;
        private Tween _shakeTween;

        public CameraShaker(Transform cameraTransform)
        {
            _cameraTransform = cameraTransform;
            _originalPosition = cameraTransform.position;
        }

        public void Shake(float duration, float magnitude)
        {
            _shakeTween?.Kill();
            _cameraTransform.position = _originalPosition;
            _shakeTween = _cameraTransform.DOShakePosition(duration, magnitude)
                .OnComplete(() => _cameraTransform.position = _originalPosition);
        }
    }
}
