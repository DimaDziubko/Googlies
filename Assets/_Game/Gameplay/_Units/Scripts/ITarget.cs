using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public interface ITarget
    {
        Faction Faction { get;}
        Transform Transform { get; }
        bool IsActive { get;}
        float BodySize { get; }
        Collider2D Collider { get;}
        void Push(Vector2 right, float impulseStrength);
        void TakeDamage(float damageToDeal);
        bool IsDead();
        Vector3 GetPosition();
    }
}