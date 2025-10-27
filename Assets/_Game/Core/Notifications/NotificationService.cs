using System;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using Unity.Notifications.iOS;
#endif
using UnityEngine;
using Zenject;

namespace _Game.Core.Notifications
{
    [UnityEngine.Scripting.Preserve]
    public class NotificationService : IInitializable, IDisposable
    {
        private const string CHANNEL_ID_DAILY_TASK = "daily_task_channel";
        private const string CHANNEL_NAME = "Daily Tasks";
        private const int NOTIFICATION_ID_DAILY = 123;
        private const int NOTIFICATION_ID_TEST = 999;

        public NotificationService()
        {
            Debug.Log("[NotificationService] Constructor called.");
            CreateChannelDailyTask();
        }

        public void Initialize()
        {
            Debug.Log("[NotificationService] Initializing...");
            //CreateChannelDailyTask();
        }

        private void CreateChannelDailyTask()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidNotificationCenter.Initialize();

                var channel = new AndroidNotificationChannel()
                {
                    Id = CHANNEL_ID_DAILY_TASK,
                    Name = CHANNEL_NAME,
                    Importance = Importance.High,
                    Description = "Notifications for daily tasks",
                };
                AndroidNotificationCenter.RegisterNotificationChannel(channel);
                Debug.Log($"[NotificationService] ✅ Android channel '{CHANNEL_NAME}' created successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationService] ❌ Failed to create Android channel: {e.Message}");
            }
#elif UNITY_IOS && !UNITY_EDITOR
            Debug.Log("[NotificationService] iOS doesn't require channels.");
#else
            Debug.Log("[NotificationService] Running in Editor - notifications disabled.");
#endif
        }

        private void CancelDailyNotification()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNotificationCenter.CancelScheduledNotification(NOTIFICATION_ID_DAILY);
#elif UNITY_IOS && !UNITY_EDITOR
            iOSNotificationCenter.RemoveScheduledNotification("daily_task_notification");
#endif
        }

        public void SendDailyTaskAvailableNotification(DateTime fireTime)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                CancelDailyNotification();
                var notification = new AndroidNotification()
                {
                    Title = "Don't miss out!",
                    Text = "You have new daily tasks available!",
                    FireTime = fireTime,
                    SmallIcon = "icon_small",
                    LargeIcon = "icon_big",
                };

                AndroidNotificationCenter.SendNotificationWithExplicitID(
                    notification,
                    CHANNEL_ID_DAILY_TASK,
                    NOTIFICATION_ID_DAILY
                );

                Debug.Log($"[NotificationService] 📅 Daily notification scheduled for {fireTime}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationService] ❌ Failed to send daily notification: {e.Message}");
            }
#endif
        }

        public void SendTestNotification(int delaySeconds = 10)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidNotificationCenter.Initialize();
                AndroidNotificationCenter.CancelScheduledNotification(NOTIFICATION_ID_TEST);

                var notification = new AndroidNotification()
                {
                    Title = "🔥 Test Notification",
                    Text = $"Yo! This is a test push sent {delaySeconds} seconds ago! Mur mur 😺",
                    FireTime = DateTime.Now.AddSeconds(delaySeconds),
                    SmallIcon = "icon_small",
                    LargeIcon = "icon_big",
                };

                AndroidNotificationCenter.SendNotificationWithExplicitID(
                    notification,
                    CHANNEL_ID_DAILY_TASK,
                    NOTIFICATION_ID_TEST
                );

                Debug.Log($"[NotificationService] ✅ Test notification scheduled for {delaySeconds}s!");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationService] ❌ Failed to send test notification: {e.Message}");
            }
#endif
        }

        public async UniTask RequestIOSNotificationPermissions()
        {
#if UNITY_IOS && !UNITY_EDITOR
            await RequestAuthorization();
#else
            await UniTask.CompletedTask;
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        private async UniTask RequestAuthorization()
        {
            try
            {
                var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
                using (var req = new AuthorizationRequest(authorizationOption, true))
                {
                    await UniTask.WaitUntil(() => req.IsCompleted);
                    Debug.Log($"[NotificationService] iOS Authorization Granted: {req.Granted}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NotificationService] iOS Auth error: {ex.Message}");
            }
        }
#endif

        public void Dispose() => Debug.Log("[NotificationService] Disposed.");
    }
}
