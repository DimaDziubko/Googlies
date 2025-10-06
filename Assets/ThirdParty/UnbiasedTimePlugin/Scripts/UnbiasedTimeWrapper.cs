using System;
using UnityEngine;

public static class UnbiasedTimeWrapper
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject unityContext;
    private static AndroidJavaClass pluginClass;
    private static bool isInitialized;

    public static void Init()
    {
        if (isInitialized) return;

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        pluginClass = new AndroidJavaClass("com.duomindevolution.unbiasedtime.UnbiasedTimePlugin");
        pluginClass.CallStatic("init", unityContext);
        isInitialized = true;
    }

    public static void UpdateFromNtp()
    {
        if (!isInitialized) Init();
        pluginClass.CallStatic("updateFromNtp", unityContext);
    }

    public static DateTime GetUnbiasedUtcNow()
    {
        if (!isInitialized) Init();
        long millis = pluginClass.CallStatic<long>("getUnbiasedUtcTimeMillis");
        return DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
    }

    public static bool IsTimeCheated()
    {
        if (!isInitialized) Init();
        return pluginClass.CallStatic<bool>("isTimeCheated");
    }
#else
    public static void Init() { }
    public static void UpdateFromNtp() { }
    public static DateTime GetUnbiasedUtcNow() => DateTime.UtcNow;
    public static bool IsTimeCheated() => false;
#endif
}