using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts._DecorAndUtils
{
    public class Plug : MonoBehaviour
    {
        public IUIFactory OriginFactory { get; set; }
    
        public void Release()
        {
            OriginFactory.Reclaim(this);
        }
    }
}
