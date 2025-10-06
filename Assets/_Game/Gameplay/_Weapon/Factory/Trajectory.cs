using UnityEngine;

namespace _Game.Gameplay._Weapon.Factory
{
    public class Trajectory : ITrajectory
    {
        private readonly AnimationCurve _curve;

        public Trajectory(AnimationCurve curve) => 
            _curve = curve;
        
        
        public Vector2 GetInitialDirection(Vector2 start, Vector2 end, float warp)
        {
            const float delta = 0.01f;
            
            Vector2 p0 = start;
            Vector2 p1 = Vector2.Lerp(start, end, delta);
            
            float y0 = _curve.Evaluate(0f) * warp;
            float y1 = _curve.Evaluate(delta) * warp;

            p0.y += y0;
            p1.y += y1;

            return (p1 - p0).normalized;
        }

        public Vector2 GetDirection(Vector2 start, Vector2 end, float t, float warp)
        {
            const float delta = 0.01f;
            float t1 = Mathf.Clamp01(t);
            float t2 = Mathf.Clamp01(t + delta);

            Vector3 p1 = GetPosition(start, end, t1, warp);
            Vector3 p2 = GetPosition(start, end, t2, warp);

            return ((Vector2)(p2 - p1)).normalized;
        }

        public Vector3 GetPosition(Vector2 start, Vector2 end, float t, float warp)
        {
            Vector2 linear = Vector2.Lerp(start, end, t);
            float heightOffset = _curve.Evaluate(t) * warp;
            return new Vector3(linear.x, linear.y + heightOffset, 0);
        }
    }
}