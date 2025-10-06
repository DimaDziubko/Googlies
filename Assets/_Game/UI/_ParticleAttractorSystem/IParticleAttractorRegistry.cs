using Coffee.UIExtensions;

namespace _Game.UI._ParticleAttractorSystem
{
    public interface IParticleAttractorRegistry
    {
        void Register(AttractableParticleType particleType, UIParticleAttractor attractor);
        bool TryGetAttractor(AttractableParticleType particleType, out UIParticleAttractor attractor);
        bool TryDeregister(AttractableParticleType eventType);
        bool Contains(AttractableParticleType eventType);
    }
}