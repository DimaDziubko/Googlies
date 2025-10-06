using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public abstract class ShopItemView : MonoBehaviour
    {
        public int Id => _id;

        [SerializeField] private int _id;

        public IUIFactory OriginFactory { get; set; }

        public void Release()
        {
            OriginFactory.Reclaim(this);
        }
        
        public abstract void Cleanup();
    }
}