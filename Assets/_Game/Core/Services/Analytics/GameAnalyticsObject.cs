using _Game.Core._GameMode;
#if gameanalytics_enabled
using GameAnalyticsSDK;
#endif
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
#if gameanalytics_enabled

    public class GameAnalyticsObject : MonoBehaviour, IGameAnalyticsATTListener
    {
        private void Start()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
            }
            else
            {
                GameAnalytics.Initialize();
            }

            InitializeGA();
            SetCustomID();
        }

        public void StartRequestforIOS()
        {
            Invoke(nameof(RequestforIOS), 1f);
        }
        public void RequestforIOS()
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        public void InitializeGA()
        {
            GameAnalyticsILRD.SubscribeMaxImpressions();
        }

        public void SetCustomID()
        {
            GameAnalytics.SetCustomId(GameMode.GetUniqUserID());
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }
        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }
    }
#endif
}