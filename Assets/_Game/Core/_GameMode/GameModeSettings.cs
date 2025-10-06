using UnityEngine;

namespace _Game.Core._GameMode
{
    public enum ConfigSource
    {
        Remote,
        Local,
        Embedded
    }
    public class GameModeSettings : MonoBehaviour
    {
        [SerializeField] private bool _testMode;
        [SerializeField] private bool _isCheatEnabled;
        [SerializeField] private ConfigSource _configSource;

        public static GameModeSettings I;

        public bool TestMode => _testMode;
        public ConfigSource ConfigSource => _configSource;
        public bool IsCheatEnabled => _isCheatEnabled;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }
    }
}
