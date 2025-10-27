using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using _Game.Core.Ads;
using AppsFlyerConnector;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class AppsFlyerAnalyticsService : IDisposable
    {
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly AppsFlyerSettings _settings;
        private readonly IAdsService _iAdsService;

        public AppsFlyerAnalyticsService(
            IMyLogger logger,
            AppsFlyerSettings settings,
            IAdsService iAdsService)
        {
            _logger = logger;
            _settings = settings;
            _iAdsService = iAdsService;
        }

        public void Initialize()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("No internet connection. Returning early.");
                return;
            }

            AppsFlyer.setIsDebug(_settings.IsDebug);
            AppsFlyer.setCustomerUserId(UnityEngine.Device.SystemInfo.deviceUniqueIdentifier);
#if UNITY_IOS && !UNITY_EDITOR
    AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif

            AppsFlyer.initSDK(_settings.DevKey, _settings.AppID, _settings);
            //_settings.SubscribeDeepLink();

            AppsFlyerPurchaseConnector.init(_settings, Store.GOOGLE);
            AppsFlyerPurchaseConnector.setIsSandbox(_settings.IsSandbox);

            AppsFlyerPurchaseConnector.setAutoLogPurchaseRevenue(  AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsAutoRenewableSubscriptions,               AppsFlyerAutoLogPurchaseRevenueOptions.AppsFlyerAutoLogPurchaseRevenueOptionsInAppPurchases);

            AppsFlyerPurchaseConnector.setPurchaseRevenueValidationListeners(true);

            AppsFlyerPurchaseConnector.build();
            AppsFlyerPurchaseConnector.startObservingTransactions();

            AppsFlyer.startSDK();

            //_iAdsService.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            Debug.Log($"Apps Flyer Inited");
        }

        void IDisposable.Dispose()
        {
            //_settings.UnsubscribeDeepLink();
            //_iAdsService.OnAdRevenuePaidEvent -= OnAdRevenuePaidEvent;
        }

        //private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    Dictionary<string, string> additionalParams = new Dictionary<string, string>();
        //    additionalParams.Add(AdRevenueScheme.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode);
        //    additionalParams.Add(AdRevenueScheme.AD_UNIT, adInfo.AdUnitIdentifier);
        //    additionalParams.Add(AdRevenueScheme.AD_TYPE, adInfo.AdFormat);
        //    additionalParams.Add(AdRevenueScheme.PLACEMENT, adInfo.Placement);
        //    var logRevenue = new AFAdRevenueData(adInfo.NetworkName, MediationNetwork.ApplovinMax, "USD", adInfo.Revenue);
        //    AppsFlyer.logAdRevenue(logRevenue, additionalParams);
        //}

        public void DebugEvent(string eventName, Dictionary<string, string> eventParameters)
        {
            Debug.Log("AppsFlyer EVENT : " + eventName);
            foreach (var kvp in eventParameters)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value}");
            }
            Debug.Log("AppsFlyer EVENT DONE : " + eventName);
        }

        public void SendAgeLevelCompleteEvent(int level, TimeSpan timeTaken)
        {
            string eventName = $"age_evolution_{level}";

            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
            { "age", $"Age {level} achieved" },
            { "time_taken_minutes", timeTaken.TotalMinutes.ToString("F1") } // Format to 1 decimal places
            };

            AppsFlyer.sendEvent(eventName, eventValues);
            _logger.Log($"LevelProgressTracker event '{eventName}' to AppsFlyer with time taken {timeTaken.TotalMinutes.ToString("F1")} ");
        }
        public void SendTimeLineLevelCompleteEvent(int level, TimeSpan timeTaken)
        {
            string eventName = $"timeline_evolution_{level}";

            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
            { "timeline", $"Timeline {level} achieved" },
            { "time_taken_minutes", timeTaken.TotalMinutes.ToString("F1") } // Format to 1 decimal places
            };

            AppsFlyer.sendEvent(eventName, eventValues);
            _logger.Log($"LevelProgressTracker event '{eventName}' to AppsFlyer with time taken {timeTaken.TotalMinutes.ToString("F1")} ");
        }
        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
        }
    }
}
