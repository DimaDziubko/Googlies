using _Game.Core.Configs.Models._WarriorsConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class WarriorObjectPositioner : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _health;
        [SerializeField, Required] private Transform _damageText;
        [SerializeField, Required] private Transform _skillEffect;

        public void Apply(WarriorObjectsPositionSettings settings)
        {
            settings.HealthBarSettings.Apply(_health);
            settings.DamageTextPointSettings.Apply(_damageText);
            settings.SkillEffectParentSettings.Apply(_skillEffect);
        }
    }
}