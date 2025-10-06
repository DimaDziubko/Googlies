using _Game.Gameplay._Units.Scripts.Movement;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._Units.Scripts
{
    public class TargetPoint : MonoBehaviour, ITarget
    {
        public IDamageable Damageable { get; set; }
        public Faction Faction { get; set; }
        public Transform Transform { get; set; }
        public float BodySize { get; set; }
        public Collider2D Collider { get; set; }
        public IPushable Pushable { get; set; }

        public void Push(Vector2 right, float impulseStrength)
        {
            if(Pushable != null)
                Pushable.Push(right, impulseStrength);
        }

        public void TakeDamage(float damageToDeal)
        {
            if(Damageable != null)
                Damageable.GetDamage(damageToDeal);
        }

        public bool IsDead()
        {
            if (Damageable != null)
                return Damageable.IsDead;
            return true;
        }

        public Vector3 GetPosition()
        {
            if(Transform != null)
                return Transform.position;
            return Vector3.zero;
        }

        public bool IsActive =>
            gameObject.OrNull()?.activeInHierarchy == true &&
            Damageable?.IsDead == false;
    }
}