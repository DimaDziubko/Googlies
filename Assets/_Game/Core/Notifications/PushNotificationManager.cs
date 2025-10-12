using System;
using Cysharp.Threading.Tasks;
#if UNITY_IOS
// using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace _Game.Core.Notifications
{
#if UNITY_IOS
    public class PushNotificationManager : IDisposable //TODO
    {
        public PushNotificationManager()
        {
            // RequestPushNotificationPermissionAsync().Forget();
        }

        // private async UniTaskVoid RequestPushNotificationPermissionAsync()
        // {
        //     // Check if the platform is iOS
        //     if (Application.platform == RuntimePlatform.IPhonePlayer)
        //     {
        //         var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;

        //         var request = new AuthorizationRequest(authorizationOption, true);

        //         await WaitForAuthorizationRequestAsync(request);

        //         Debug.Log($"Push notifications request status: {request.Granted}.");

        //         // Check the actual notification settings to ensure the correct status
        //         var settings = iOSNotificationCenter.GetNotificationSettings();

        //         string status = settings.AuthorizationStatus == AuthorizationStatus.Authorized ? "granted" : "denied";

        //         Debug.Log($"Push notifications permission confirmed as {status}.");

        //         LogPermissionStatusToAnalytics(status);

        //         request.Dispose();
        //     }
        // }

        // private async UniTask WaitForAuthorizationRequestAsync(AuthorizationRequest request)
        // {
        //     while (!request.IsFinished)
        //     {
        //         await UniTask.Yield(); // Await the next frame until the request is finished
        //     }
        // }

        // private async void LogPermissionStatusToAnalytics(string status)
        // {
        //     // Delay logging for consistency
        //     await UniTask.Delay(TimeSpan.FromSeconds(3));

        //     // Debug.Log("Push notification status: " + status);
        // }

        public void Dispose()
        {
        }
}
#endif
}
