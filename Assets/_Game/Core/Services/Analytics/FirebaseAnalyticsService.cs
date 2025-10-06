using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Assets._Game.Core.UserState;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Installations;
using Firebase.Messaging;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class FirebaseAnalyticsService : IAnalyticsService, IDisposable
    {
        private FirebaseApp _app;
        private bool _isFirebaseInitialized = false;
        private string UniqueID { get; set; }

        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IAdsService _adsService;
        
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;

        public FirebaseAnalyticsService(
            IMyLogger logger,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IAdsService adsService)
        {
            _logger = logger;
            _userContainer = userContainer;
            _adsService = adsService;
            gameInitializer.RegisterAsyncInitialization(Init);

            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        private async UniTask Init()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;

                _app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
                SubscribeTopic();
                await FirebaseMessaging.RequestPermissionAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("FCM Permission Granted.");
                    }
                    else
                    {
                        Debug.LogError("FCM Permission Denied.");
                    }
                });

                InnerInit();
                SetUniqID();

                _isFirebaseInitialized = true;
                _logger.Log("Firebase Initialized Successfully");
            }
            else
            {
                _logger.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }

            Subscribe();
        }

        private void Subscribe()
        {
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
        }

        private void SubscribeTopic()
        {
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/TestTopic");
        }

        private void InnerInit() => _adsService.OnAdRevenuePaidEvent += LogAdPurchase;

        private void LogAdPurchase(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            double revenue = adInfo.Revenue;
            if (revenue > 0)
            {
                string countryCode = MaxSdk.GetSdkConfiguration()
                    .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
                string
                    networkName =
                        adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
                string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
                string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
                string networkPlacement =
                    adInfo.NetworkPlacement; // The placement ID from the network that showed the ad

                var impressionParameters = new[]
                {
                    new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                    new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
                    new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
                    new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
                    new Firebase.Analytics.Parameter("value", revenue),
                    new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
                };

                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            }
        }

        private async void SetUniqID()
        {
            UniqueID = UnityEngine.Device.SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
            }
            FirebaseAnalytics.SetUserId(UniqueID);
            _logger.Log($"Generated fallback Unique ID: {UniqueID}");
            GetUniqIdAsync();
        }

        void IDisposable.Dispose()
        {
            Unsubscribe();

            _app?.Dispose();
        }

        private void Unsubscribe()
        {
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleChanged;
            _adsService.OnAdRevenuePaidEvent -= LogAdPurchase;
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        private void OnCompletedBattleChanged()
        {
            if (!IsComponentsReady()) return;

            if (BattleStatistics.BattlesCompleted == 1 &&
                TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                SendEvent("first_build_success");
                _logger.Log("first_build_success");
            }

            else if (BattleStatistics.BattlesCompleted == 1 &&
                     !TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER))
            {
                SendEvent("first_build_failed");
                _logger.Log("first_build_failed");
            }
        }
        
        private bool IsComponentsReady() =>
            _isFirebaseInitialized || _userContainer != null;

        private void SendEvent(string eventName, Dictionary<string, object> eventData)
        {
            if (_isFirebaseInitialized)
            {
                var analyticsEvent = new AnalyticsEvent(eventName);
                foreach (var kv in eventData)
                {
                    analyticsEvent.AddParameter(kv.Key, kv.Value);
                }

                analyticsEvent.Send();
            }
            else
            {
                _logger.LogWarning($"Firebase app not initialized. Cannot log event: {eventName}");
            }
        }

        public void SendEvent(string eventName)
        {
            if (_isFirebaseInitialized)
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                _logger.LogWarning($"Firebase app not initialized. Cannot log event: {eventName}");
            }
        }

        private async UniTaskVoid GetUniqIdAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            try
            {
                var token = await FirebaseInstallations.DefaultInstance.GetTokenAsync(forceRefresh: true);
                var uniqueID = token;
                Debug.Log($"Installations token {token}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error getting UniqueID: {ex.Message}");
            }
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log($"FCM Token: {token.Token}");
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //CreateChannelFCM();

            Debug.Log($"Message received: {e.Message.From}");
            if (e.Message.Notification != null)
            {
                Debug.Log($"Notification Title: {e.Message.Notification.Title}");
                Debug.Log($"Notification Body: {e.Message.Notification.Body}");
            }
        }
    }
}