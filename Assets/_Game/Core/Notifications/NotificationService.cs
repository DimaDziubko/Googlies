using System;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
//using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace _Game.Core.Notifications
{
    public class NotificationService : IDisposable
    {
        private const string ID_DAILYTASK = "daily_task";
        private const int ID = 123;

        public NotificationService()
        {
            CreateChannelDailyTask();
        }

        private void CancelDailyNotification()
        {
#if UNITY_ANDROID
            //if (AndroidNotificationCenter.CheckScheduledNotificationStatus(ID) == NotificationStatus.Scheduled)
            //{
            //    AndroidNotificationCenter.CancelScheduledNotification(ID);
            //}
#elif UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(ID_DAILYTASK);
#endif
        }

        public void SendDailyTaskAvailableNotification(DateTime fireTime)
        {
#if UNITY_ANDROID
            CancelDailyNotification();
            //AndroidNotificationCenter.CancelAllNotifications();
            //var notification = new AndroidNotification()
            //{
            //    Title = "Don’t miss out!",
            //    Text = "You have new daily tasks available!",
            //    FireTime = fireTime,
            //    SmallIcon = "icon_small",
            //    LargeIcon = "icon_big",
            //};
            //AndroidNotificationCenter.SendNotificationWithExplicitID(notification, ID_DAILYTASK, ID);

#elif UNITY_IOS
            CancelDailyNotification();
            TimeSpan timeSpan = fireTime - DateTime.UtcNow;
            if (timeSpan.TotalSeconds < 1) return;
            var notification = new iOSNotification()
            {
                Identifier = "daily_task_notification",
                Title = "Don`t miss out!",
                Body = "You have new daily tasks available!",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "daily_task_category",
                ThreadIdentifier = "game_notifications",
                Trigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = timeSpan,
                    Repeats = false
                }
            };
            iOSNotificationCenter.ScheduleNotification(notification);
            Debug.Log($"[iOS Notification] Scheduling notification with TimeInterval: {timeSpan}.");
#endif
        }

        private void CreateChannelDailyTask()
        {
#if UNITY_ANDROID
            //var channel = new AndroidNotificationChannel()
            //{
            //    Id = ID_DAILYTASK,
            //    Name = "Daily Task",
            //    Importance = Importance.Default,
            //    Description = "Generic notifications",
            //};
            //AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
            // For iOS, Channels are not required, but we need to handle permissions
#endif
        }

        public async UniTask RequestIOSNotificationPermissions()
        {
#if UNITY_IOS
            await RequestAuthorization();
#endif
        }

#if UNITY_IOS
        private async UniTask RequestAuthorization()
        {
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;

            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                try
                {
                    // Asynchronously wait until the request is finished
                    await UniTask.WaitUntil(() => req.IsFinished);

                    // Build the result string
                    string res = "\n RequestAuthorization:";
                    res += "\n finished: " + req.IsFinished;
                    res += "\n granted :  " + req.Granted;
                    res += "\n error:  " + req.Error;
                    res += "\n deviceToken:  " + req.DeviceToken;
                    Debug.Log(res);

                    // You can handle the result here
                    if (req.Granted)
                    {
                        // Permission granted
                        // Schedule notifications or proceed accordingly
                    }
                    else
                    {
                        // Permission denied
                        // Handle the denial
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error during authorization request: {ex.Message}");
                }
            }
        }
#endif
        public void Dispose()
        {
        }
    }
}