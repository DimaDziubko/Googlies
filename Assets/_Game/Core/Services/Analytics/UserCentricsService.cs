using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Gameplay.Common;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Usercentrics;
using UnityEngine;
using Zenject;

namespace _Game.Core.Services.Analytics
{
    public class UserCentricsService : IInitializable, IDisposable
    {
        [Inject] private MaxAdsService _maxAdsService;
        [Inject] private AppsFlyerAnalyticsService _appsFlyerAnalyticsService;
        [Inject] private ICoroutineRunner _coroutineRunner;
#if UNITY_IOS
        [Inject] private IOSTrackIDFAService _iOSTrackIDFAService;
#endif

        private bool _isInitialized = false;
        private bool _isRetryingConnection = false;
        private Coroutine _connectionRetryCoroutine;

        void IInitializable.Initialize()
        {
#if UNITY_EDITOR
            InitGame();
            return;
#endif

            TryInitializeUsercentrics();
        }

        private void TryInitializeUsercentrics()
        {
            if (_isInitialized)
                return;

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("[USERCENTRICS] Internet available, initializing...");

                Usercentrics.Instance.Initialize((status) =>
                {
                    _isInitialized = true;
                    StopConnectionRetry();

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
                    Debug.Log("[USERCENTRICS] AutoInitialize error: " + errorMessage);
                    InitGame();
                });
            }
            else
            {
                Debug.Log("[USERCENTRICS] No internet connection, starting retry mechanism...");
                InitGame(); // Ініціалізуємо гру без згоди
                StartConnectionRetry();
            }
        }

        private void StartConnectionRetry()
        {
            if (_isRetryingConnection || _isInitialized)
                return;

            _isRetryingConnection = true;
            _connectionRetryCoroutine = _coroutineRunner.StartCoroutine(RetryConnectionCoroutine());
        }

        private void StopConnectionRetry()
        {
            if (_connectionRetryCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_connectionRetryCoroutine);
                _connectionRetryCoroutine = null;
            }
            _isRetryingConnection = false;
        }

        private IEnumerator RetryConnectionCoroutine()
        {
            while (!_isInitialized && Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return new WaitForSeconds(2f); // Перевіряємо кожні 2 секунди
            }

            if (!_isInitialized && Application.internetReachability != NetworkReachability.NotReachable)
            {
                Debug.Log("[USERCENTRICS] Internet connection restored, retrying initialization...");

                // Перезапускаємо SDK як рекомендовано
                RestartUsercentricsSDK();
            }

            _isRetryingConnection = false;
        }

        private void RestartUsercentricsSDK()
        {
            // Перезапуск SDK при відновленні з'єднання
            try
            {
                Usercentrics.Instance.Initialize((status) =>
                {
                    _isInitialized = true;
                    Debug.Log("[USERCENTRICS] Successfully reinitialized after connection restore");

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
                    Debug.Log("[USERCENTRICS] Reinitialize error: " + errorMessage);
                    // Можна спробувати ще раз через деякий час
                    _coroutineRunner.StartCoroutine(DelayedRetry());
                });
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[USERCENTRICS] Exception during SDK restart: " + ex.Message);
            }
        }

        private IEnumerator DelayedRetry()
        {
            yield return new WaitForSeconds(5f);
            if (!_isInitialized)
            {
                StartConnectionRetry();
            }
        }

        private void InitGame()
        {
#if UNITY_ANDROID
            _maxAdsService.Init();
            _appsFlyerAnalyticsService.Initialize();
#endif
#if UNITY_IOS
            if (Application.internetReachability != NetworkReachability.NotReachable)
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
                        MaxSdk.SetHasUserConsent(serviceConsent.status);
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
            StopConnectionRetry();
        }
    }
}