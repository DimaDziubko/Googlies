using Coffee.UIExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._ParticleAttractorSystem
{
    public class AttractorWrapper : MonoBehaviour
    {
        [SerializeField] private AttractableParticleType _particleAttractableType;
        [SerializeField, Required] private UIParticleAttractor _attractor;
        
        public AttractableParticleType ParticleAttractableType => _particleAttractableType;
        public UIParticleAttractor Attractor => _attractor;
    }
}