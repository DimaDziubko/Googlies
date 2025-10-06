using UnityEngine;

namespace _Game.UI._Shop.Scripts._DecorAndUtils
{
    public class Delimiter : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
