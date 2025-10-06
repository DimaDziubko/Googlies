using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class AppsFlyerSettings : MonoBehaviour, IAppsFlyerConversionData
    {
        private const string AppsFlyerApiEndpoint = "https://hq.appsflyer.com/api/v4/partners/<app-id>/attribution";


        [SerializeField] private bool _isDebug;
        [SerializeField] private string _devKeyAndroid;
        [SerializeField] private string _devKeyIOS;
        [SerializeField] private bool _isGetConversionData;
        [SerializeField] private bool _isSandbox;
        /// <summary>
        /// For IOS
        /// </summary>
        [SerializeField] private string _appID;

#if UNITY_IOS
        public string DevKey => _devKeyIOS;
#else // UNITY_ANDROID
        public string DevKey => _devKeyAndroid;
#endif
        public bool IsDebug => _isDebug;
        public bool IsGetConversionData => _isGetConversionData;
        public bool IsSandbox => _isSandbox;
        public string AppID => _appID;


        public void SubscribeDeepLink()
        {
            AppsFlyer.OnDeepLinkReceived += OnDeepLink;
        }

        public void UnsubscribeDeepLink()
        {
            AppsFlyer.OnDeepLinkReceived -= OnDeepLink;
        }

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            if (conversionData == null)
            {
                Debug.LogError("Conversion data is null.");
                return;
            }

            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        }
        public void onConversionDataFail(string error)
        {
        }
        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        }

        private void OnDeepLink(object sender, EventArgs args)
        {
            var deepLinkEventArgs = args as DeepLinkEventsArgs;

            if (deepLinkEventArgs == null)
            {
                Debug.LogError("Invalid deep link arguments.");
                return;
            }

            switch (deepLinkEventArgs.status)
            {
                case DeepLinkStatus.FOUND:
                    if (deepLinkEventArgs.isDeferred())
                    {
                        AppsFlyer.AFLog("OnDeepLink", "This is a deferred deep link.");
                    }
                    else
                    {
                        AppsFlyer.AFLog("OnDeepLink", "This is a direct deep link.");
                    }

                    // Parse deep link data
                    Dictionary<string, object> deepLinkParamsDictionary = null;

#if UNITY_IOS && !UNITY_EDITOR
                    if (deepLinkEventArgs.deepLink.ContainsKey("click_event") && deepLinkEventArgs.deepLink["click_event"] != null)
                    {
                        deepLinkParamsDictionary = deepLinkEventArgs.deepLink["click_event"] as Dictionary<string, object>;
                    }
#elif UNITY_ANDROID && !UNITY_EDITOR
                    deepLinkParamsDictionary = deepLinkEventArgs.deepLink as Dictionary<string, object>;
#endif

                    if (deepLinkParamsDictionary != null)
                    {
                        ProcessDictionaryData(deepLinkParamsDictionary);
                    }
                    else
                    {
                        Debug.LogWarning("Deep link parameters are null.");
                    }
                    break;

                case DeepLinkStatus.NOT_FOUND:
                    AppsFlyer.AFLog("OnDeepLink", "Deep link not found. Fetching data from AppsFlyer API...");
                    FetchAttributionFromServer();
                    break;

                default:
                    AppsFlyer.AFLog("OnDeepLink", "Deep link error.");
                    break;
            }
        }

        private async void FetchAttributionFromServer()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", DevKey);

                try
                {
                    var response = await client.GetAsync(AppsFlyerApiEndpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        Debug.Log($"Attribution data fetched from server: {data}");

                        var attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(data);
                        if (attributionDataDictionary != null)
                        {
                            ProcessDictionaryData(attributionDataDictionary);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to fetch attribution data: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception while fetching attribution data: {ex.Message}");
                }
            }
        }

        private void ProcessDictionaryData(Dictionary<string, object> data)
        {
            string af_status = data.ContainsKey("af_status") ? data["af_status"].ToString() : "N/A";
            string media_source = data.ContainsKey("media_source") ? data["media_source"].ToString() : "N/A";
            string campaign = data.ContainsKey("campaign") ? data["campaign"].ToString() : "N/A";
            string campaign_id = data.ContainsKey("campaign_id") ? data["campaign_id"].ToString() : "N/A";
            string adset = data.ContainsKey("adset") ? data["adset"].ToString() : "N/A";
            string adset_id = data.ContainsKey("adset_id") ? data["adset_id"].ToString() : "N/A";
            string adgroup = data.ContainsKey("adgroup") ? data["adgroup"].ToString() : "N/A";
            string adgroup_id = data.ContainsKey("adgroup_id") ? data["adgroup_id"].ToString() : "N/A";
            string ad_id = data.ContainsKey("ad_id") ? data["ad_id"].ToString() : "N/A";

            Debug.Log($"AFstatus: {af_status}");
            Debug.Log($"Media Source: {media_source}");
            Debug.Log($"Campaign: {campaign}");
            Debug.Log($"CampaignID: {campaign_id}");
            Debug.Log($"Adset: {adset}");
            Debug.Log($"AdsetID: {adset_id}");
            Debug.Log($"Adgroup: {adgroup}");
            Debug.Log($"AdgroupID: {adgroup_id}");
            Debug.Log($"AdID: {ad_id}");
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
        public void didReceivePurchaseRevenueValidationInfo(string validationInfo)
        {
            AppsFlyer.AFLog("didReceivePurchaseRevenueValidationInfo", validationInfo);
        }
    }
}