using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

namespace _Game.Utils
{
    public class GeoLocationChecker : MonoBehaviour
    {
        public static GeoLocationChecker I;

        private bool _isInEURegion;

        public static bool IsInEURegion => I._isInEURegion;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }

        private void Start()
        {
            _isInEURegion = false;
            //StartCoroutine(GetLocation());
        }

        private IEnumerator GetLocation()
        {
            string url = "http://ip-api.com/json";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    ParseLocationData(webRequest.downloadHandler.text);
                    //Debug.Log("ParseLocationData: " + webRequest.downloadHandler.text);
                }
            }
        }

        private void ParseLocationData(string jsonData)
        {
            LocationData locationData = JsonUtility.FromJson<LocationData>(jsonData);
            if (locationData.CountryCode == "EU")
            {
                SetEurope();
            }
        }

        private void SetEurope()
        {
            _isInEURegion = true;
        }

        [System.Serializable]
        public class LocationData
        {
            public string CountryCode;
        }
    }
}
