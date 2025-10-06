using UnityEngine;

namespace _Game.UI._Hud
{
    public class Curtain : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
