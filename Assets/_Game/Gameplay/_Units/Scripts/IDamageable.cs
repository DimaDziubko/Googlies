namespace _Game.Gameplay._Units.Scripts
{
    public interface IDamageable
    {
        bool IsDead { get; }
        void GetDamage(float damage);
    }
}