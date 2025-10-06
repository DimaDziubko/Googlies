using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class ParticlesMoveToTarget : MonoBehaviour
    {
        public ParticleSystem particles;
        public Transform target;
        public float delay = 1.0f;
        private ParticleSystem.Particle[] particleArray;

        void Start()
        {
            particles = GetComponent<ParticleSystem>();
            particleArray = new ParticleSystem.Particle[particles.main.maxParticles];
        }

        void Update()
        {
            int numParticlesAlive = particles.GetParticles(particleArray);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                float particleAge = particleArray[i].startLifetime - particleArray[i].remainingLifetime;
                if (particleAge > delay)
                {
                    Vector3 directionToTarget = (target.position - particleArray[i].position).normalized;
                    particleArray[i].velocity = directionToTarget * particleArray[i].velocity.magnitude;
                }
            }

            particles.SetParticles(particleArray, numParticlesAlive);
        }
    }
}
