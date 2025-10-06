using UnityEngine;

namespace _Game.Core.Ads
{
    public class MaxHelper : MonoBehaviour
    {
        [SerializeField] private bool _isDebugAdMode;

        public static MaxHelper I;

        public bool IsDebugAdMode => _isDebugAdMode;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }
    }
}
