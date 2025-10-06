namespace _Game.Gameplay._Weapon.Scripts
{
    public class WeaponDamageBoostDecorator : WeaponDataDecorator
    {
        private readonly float _damageMultiplier;

        public WeaponDamageBoostDecorator(IWeaponData weaponData, float damageMultiplier) : base(weaponData)
        {
            _damageMultiplier = damageMultiplier;
        }

        public override float Damage =>  base.Damage * DamageBoost;
        public override float DamageBoost => _damageMultiplier * base.DamageBoost;

    }
    
    public class WeaponDamageDecorator : WeaponDataDecorator
    {
        private readonly float _damageMultiplier;

        public WeaponDamageDecorator(IWeaponData weaponData, float damageMultiplier) : base(weaponData)
        {
            _damageMultiplier = damageMultiplier;
        }

        public override float Damage => base.Damage * _damageMultiplier;

    }
}