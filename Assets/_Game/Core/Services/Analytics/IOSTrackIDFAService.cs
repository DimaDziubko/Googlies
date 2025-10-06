using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using _Game.Core._GameInitializer;
using Zenject;
using _Game.Core.Ads.ApplovinMaxAds;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace _Game.Core.Services.Analytics
{
#if UNITY_IOS
    public class IOSTrackIDFAService : IDisposable
    {

        public event Action<ATTrackingStatusBinding.AuthorizationTrackingStatus> OnIDFASendEvent;

        private string _idfaStr;
        //private IGameInitializer _gameInitializer;
        private AppsFlyerAnalyticsService _appsFlyerAnalyticsService;
        [Inject] private MaxAdsService _maxAdsService;

        public IOSTrackIDFAService(
            AppsFlyerAnalyticsService appsFlyerAnalyticsService
        )
        {
            //_gameInitializer = gameInitializer;
            _appsFlyerAnalyticsService = appsFlyerAnalyticsService;
        }

        public void InitIDFA()
        {
            StartInit().Forget();
        }

        private async UniTaskVoid StartInit()
        {
            // Delay for 1 second
            //await UniTask.Delay(TimeSpan.FromSeconds(1));
            Debug.Log("IDFA StartInit: ");
            await TrackIDFAAsync();
        }

        private async UniTask TrackIDFAAsync()
        {
            // Request IDFA authorization
            RequestIDFAAuthorization();

            // Wait until the user makes a decision (authorized, denied, or restricted)
            while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                await UniTask.Yield(); // Wait for the next frame
            }

            // Retrieve the IDFA once authorization status is available
            await GetAdvertisingIDAsync();

            // Log the IDFA or the authorization status
            Debug.Log("IDFA iOS: " + _idfaStr);

            // Call the function after IDFA is obtained
            OnIDFAAcquired();
        }

        private async UniTask GetAdvertisingIDAsync()
        {
            _idfaStr = "";

            // Get the current tracking authorization status
            var currentStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            // If tracking is not authorized, set the status as a string
            if (currentStatus != ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
            {
                _idfaStr = $"{currentStatus}";
            }
            else
            {
                // If authorized, retrieve the IDFA asynchronously
                _idfaStr = await RequestAdvertisingIdentifierAsync();
            }

            // Log the IDFA status
            OnIDFASendEvent?.Invoke(currentStatus);
        }

        private UniTask<string> RequestAdvertisingIdentifierAsync()
        {
            var tcs = new UniTaskCompletionSource<string>();

            Application.RequestAdvertisingIdentifierAsync((idfa, trackingEnabled, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    tcs.TrySetResult(idfa);
                }
                else
                {
                    tcs.TrySetException(new Exception(error));
                }
            });

            return tcs.Task;
        }

        private void RequestIDFAAuthorization()
        {
            // Get the current tracking authorization status
            var currentStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            // If the status is not determined, request authorization
            if (currentStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
        }

        private void OnIDFAAcquired()
        {
            // This function can be customized to trigger actions after IDFA retrieval.
            Debug.Log("IDFA has been successfully acquired: " + _idfaStr);
            _appsFlyerAnalyticsService.Initialize();
            _maxAdsService.Init();
        }
        public void Dispose()
        {
            //_gameInitializer.OnPostInitialization -= OnPostInitializationHandler;
        }
    }
#endif
}
