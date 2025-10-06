using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class ShootData
    {
        public Faction Faction;
        public ITarget Target;
        public Vector3 LaunchPosition;
        public Vector3 ProjectileScale;
        public Quaternion LaunchRotation;
        public IWeaponData WeaponData;
    }
}