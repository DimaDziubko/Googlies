using UnityEngine;

namespace _Game.Core.Services._Camera
{
    public interface IWorldCameraService 
    {
        public Transform MainCameraTransform { get; }
        Camera MainCamera { get; }
        Camera UICameraOverlay { get; }
        float CameraHeight { get; }
        float CameraWidth { get; }
        Ray ScreenPointToRay(Vector3 mousePosition);
        Vector3 ScreenToWorldPoint(Vector3 vector3);
        void DisableMainCamera();
        void EnableMainCamera();
        void CalculateSize();
        void Shake(float duration, float magnitude);
        bool IsVisibleOnCameraX(Vector3 position);
    }
}