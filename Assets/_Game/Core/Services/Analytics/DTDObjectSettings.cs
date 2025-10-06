using _Game.Core.Services.Analytics.Data;
using DevToDev.Analytics;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class DTDObjectSettings : MonoBehaviour
    {
        [SerializeField] private bool _isDebugLog;
        [SerializeField] private DTDLogLevel _dtdLogLevel;
        [Space][SerializeField] private DTDCredentials[] _credentials;


        public DTDCredentials[] Credentials => _credentials;

        public bool IsDebugLog => _isDebugLog;
        public DTDLogLevel DTDLogLevel => _dtdLogLevel;
    }
}