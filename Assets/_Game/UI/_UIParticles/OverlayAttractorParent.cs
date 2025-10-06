using _Game.Gameplay._Coins.Factory;
using _Game.UI._ParticleAttractorSystem;
using UnityEngine;

namespace _Game.UI._UIParticles
{
    public class OverlayAttractorParent : MonoBehaviour
    {
        [SerializeField] private AttractorWrapper _coinsAttractor;
        [SerializeField] private AttractorWrapper _gemsAttractor;
        [SerializeField] private AttractorWrapper _skillPotionAttractor;
        
        public AttractorWrapper CoinsAttractor => _coinsAttractor;
        public AttractorWrapper GemsAttractor => _gemsAttractor;
        public AttractorWrapper SkillPotionAttractor => _skillPotionAttractor;

        public ICoinFactory OriginFactory { get; set; }

    }
}