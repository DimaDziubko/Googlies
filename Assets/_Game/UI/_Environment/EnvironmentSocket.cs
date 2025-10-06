using _Game.UI._Environment.Factory;
using UnityEngine;

namespace _Game.UI._Environment
{
    public class EnvironmentSocket : MonoBehaviour
    {
        [SerializeField] private Canvas _environmentCanvas;
        [SerializeField] private Transform _environmentAnchor;

        public Transform EnvironmentAnchor => _environmentAnchor;
        public IEnvironmentFactory OriginFactory { get; set; }

        public void Construct(Camera camera)
        {
            _environmentCanvas.worldCamera = camera;
        }
    }
}
