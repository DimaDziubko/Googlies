//using Facebook.Unity;
using UnityEngine;
using Zenject;

namespace _Game.Core._Facebook
{
    public class FacebookInitializer : IInitializable
    {
        public void Initialize()
        {
            //if (!FB.IsInitialized)
            //{
            //    FB.Init(InitCallback, OnHideUnity);
            //}
            //else
            //{
            //    FB.ActivateApp();
            //}
        }

        private void InitCallback()
        {
            //if (FB.IsInitialized)
            //{
            //    FB.ActivateApp();
            //}
            //else
            //{
            //    Debug.Log("Failed to Initialize the Facebook SDK");
            //}
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}