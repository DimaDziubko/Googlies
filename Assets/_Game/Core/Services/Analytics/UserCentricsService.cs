using System;
using System.Collections.Generic;
using _Game.Core.Ads.ApplovinMaxAds;
using Firebase.Analytics;
using Unity.Usercentrics;
using UnityEngine;
using Zenject;

namespace _Game.Core.Services.Analytics
{
    public class UserCentricsService : IInitializable, IDisposable
    {
        //[Inject] private IGameInitializer _gameInitializer;
        [Inject] private MaxAdsService _maxAdsService;
        [Inject] private AppsFlyerAnalyticsService _appsFlyerAnalyticsService;

#if UNITY_IOS
        [Inject] private IOSTrackIDFAService _iOSTrackIDFAService;
#endif

        void IInitializable.Initialize()
        {
#if UNITY_EDITOR
            InitGame();
            return;
#endif

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Usercentrics.Instance.Initialize((status) =>
                {
                    if (status.shouldCollectConsent)
                    {
                        ShowFirstLayer();
                    }
                    else
                    {
                        ApplyConsent(status.consents);
                    }
                },
                (errorMessage) =>
                {
                    Debug.Log("[USERCENTRICS] AutoInitialize is " + errorMessage);
                    InitGame();
                });
            }
            else
            {
                InitGame();
            }
        }

        private void InitGame()
        {
#if UNITY_ANDROID
            _maxAdsService.Init();
            _appsFlyerAnalyticsService.Initialize();
#endif
#if UNITY_IOS
            _iOSTrackIDFAService?.InitIDFA();
#endif
        }

        private void ApplyConsent(List<UsercentricsServiceConsent> consents)
        {
            foreach (var serviceConsent in consents)
            {
                switch (serviceConsent.templateId)
                {
                    case "fHczTMzX8": // AppLovin
                        //MaxSdk.SetHasUserConsent(serviceConsent.status);
                        Debug.Log("MaxSdk SetHasUserConsent " + serviceConsent.status);
                        break;

                    case "42vRvlulK96R-F": // Firebase
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(serviceConsent.status);
                        Debug.Log("Firebase Analytics consent status: " + serviceConsent.status);
                        break;
                    default:
                        Debug.Log($"Unknown consent template ID: {serviceConsent.templateId}");
                        break;
                }
            }

            InitGame();
        }

        private void ShowFirstLayer()
        {
            Usercentrics.Instance.ShowFirstLayer((userResponse) =>
            {
                ApplyConsent(userResponse.consents);
            });
        }
        public void Dispose()
        {
            // Unsubscribe from events
        }
    }
}
