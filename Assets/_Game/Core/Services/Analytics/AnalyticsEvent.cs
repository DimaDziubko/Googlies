using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class AnalyticsEvent
    {
        private readonly string _eventName;
        private readonly List<Parameter> _parameters = new();

        public AnalyticsEvent(string eventName)
        {
            _eventName = eventName;
        }

        public AnalyticsEvent AddParameter(string key, object value)
        {
            _parameters.Add(new Parameter(key, value.ToString()));
            return this;
        }

        public void Send()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                Debug.LogWarning("Firebase not initialized. Cannot send event.");
                return;
            }

            FirebaseAnalytics.LogEvent(_eventName, _parameters.ToArray());
        }
            
    }
}