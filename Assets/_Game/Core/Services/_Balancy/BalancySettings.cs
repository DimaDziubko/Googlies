using _Game.Core._GameMode;
using Balancy;
using UnityEngine;

namespace _Game.Core.Services._Balancy
{
    public class BalancySettings : MonoBehaviour
    {
        [SerializeField] private string _gameID;
        [SerializeField] private string _publicKey;

        public string GameID => _gameID;
        public string PublicKey => _publicKey;
        public Constants.Environment Environment => GameModeSettings.I.BalancyConfig;
    }
}