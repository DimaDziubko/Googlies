using UnityEngine;

namespace _Game.Gameplay._Weapon.Factory
{
    public interface ITrajectory
    {
        Vector2 GetInitialDirection(Vector2 start, Vector2 end, float warp);
        Vector3 GetPosition(Vector2 start, Vector2 end, float t, float warp);
        Vector2 GetDirection(Vector2 start, Vector2 end, float t, float warp);
    }
}