namespace _Game.Gameplay._Weapon._Projectile
{
    public class DirectProjectileImpact : ProjectileImpact
    {
        public override void Hit()
        {
            if (_target is { IsActive: true }) 
                _target.TakeDamage(_weaponData.Damage);
            _callback?.Invoke();
        }
    }
}