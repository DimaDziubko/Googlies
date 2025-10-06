using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SimpleVfxController : MonoBehaviour
    {
        [SerializeField, Required] private ParticleSystem _particles;    
        
        public void PlayParticleEffect()
        {
            _particles.Play();
        }
    }
}