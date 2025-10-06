using _Game.Gameplay._Coins.Factory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class FlyingCurrencyParent : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private RectTransform _transform;

        public ICoinFactory OriginFactory { get; set; }

        public RectTransform Transform => _transform;

        public void Construct(Camera camera)
        {
            _canvas.worldCamera = camera;
        }
    }
}