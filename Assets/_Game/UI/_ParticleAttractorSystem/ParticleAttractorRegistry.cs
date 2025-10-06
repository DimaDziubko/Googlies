using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Sirenix.OdinInspector;

namespace _Game.UI._ParticleAttractorSystem
{
    public class ParticleAttractorRegistry : IParticleAttractorRegistry, IDisposable
    {
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<AttractableParticleType, UIParticleAttractor> _registry = new();
        public void Register(AttractableParticleType particleType, UIParticleAttractor attractor) => 
            _registry.TryAdd(particleType, attractor);
        public bool TryGetAttractor(AttractableParticleType particleType, out UIParticleAttractor attractor) => 
            _registry.TryGetValue(particleType, out attractor);
        public bool TryDeregister(AttractableParticleType eventType) => 
            _registry.Remove(eventType);
        public bool Contains(AttractableParticleType eventType) => 
            _registry.ContainsKey(eventType);
        private void Clear() => _registry.Clear();
        void IDisposable.Dispose() => Clear();
    }
}