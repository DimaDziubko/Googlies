using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SkillsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        public Transform Transform => _transform;
    }
}