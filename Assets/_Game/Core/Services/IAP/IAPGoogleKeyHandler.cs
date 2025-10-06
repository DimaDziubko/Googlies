using UnityEngine;

namespace _Game.Core.Services.IAP
{
    public class IAPGoogleKeyHandler : MonoBehaviour
    {
        [SerializeField] private string _publicGoogleKey;

        public static IAPGoogleKeyHandler I;

        public string PublicGoogleKey => _publicGoogleKey;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }
    }
}