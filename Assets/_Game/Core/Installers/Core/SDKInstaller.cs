using _Game.Core._Facebook;
using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
#if UNITY_IOS
using _Game.Core.Notifications;
#endif

namespace _Game.Core.Installers.Core
{
    public class SDKInstaller : MonoInstaller
    {
        [SerializeField, Required] private AppsFlyerSettings _appsFlyerSettings;
        [SerializeField, Required] private DTDObjectSettings _dtdObjectSettings;
        [SerializeField, Required] private BalancySettings _settings;

        public override void InstallBindings()
        {
            BindAdsService();
            BindAppsFlyerAnalyticsService();
            BindDev2DevAnalyticsService();
            BindDev2DevEventSender();
            BindAnalyticsService();
#if UNITY_IOS
            BindIOSIDFAService();
            BindPushService();
#endif
            BindFacebookInstaller();
            BindUserCentricsService();
            BindBalancySDK();
            BindBalancyProfileCleaner();
        }

        private void BindBalancyProfileCleaner() =>
            Container.BindInterfacesAndSelfTo<BalancyProfileChecker>()
                .AsSingle()
                .NonLazy();

        private void BindBalancySDK()
        {
            Container.BindInterfacesAndSelfTo<BalancySDKService>()
                .AsSingle()
                .WithArguments(_settings)
                .NonLazy();
        }

        private void BindDev2DevEventSender() =>
            Container.BindInterfacesAndSelfTo<DTDEventSender>()
                .AsSingle()
                .NonLazy();
        
        private void BindAppsFlyerAnalyticsService() =>
            Container.BindInterfacesAndSelfTo<AppsFlyerAnalyticsService>()
                .AsSingle()
                .WithArguments(_appsFlyerSettings)
                .NonLazy();

        private void BindUserCentricsService() =>
            Container
                .BindInterfacesAndSelfTo<UserCentricsService>()
                .AsSingle()
                .NonLazy();

        private void BindDev2DevAnalyticsService() =>
            Container.BindInterfacesAndSelfTo<AnalyticsService>()
                .AsSingle()
                .WithArguments(_dtdObjectSettings)
                .NonLazy();

        private void BindAnalyticsService() =>
            Container.BindInterfacesAndSelfTo<FirebaseAnalyticsService>().AsSingle();

        private void BindAdsService() =>
            Container
                .BindInterfacesAndSelfTo<MaxAdsService>()
                .AsSingle();

#if UNITY_IOS
        private void BindIOSIDFAService() =>
            Container
                .BindInterfacesAndSelfTo<IOSTrackIDFAService>()
                .AsSingle()
                .NonLazy();

        private void BindPushService() =>
            Container
            .BindInterfacesAndSelfTo<PushNotificationManager>()
            .AsSingle()
            .NonLazy();
#endif
        
        private void BindFacebookInstaller() =>
            Container.BindInterfacesAndSelfTo<FacebookInitializer>().AsSingle().NonLazy();
    }
}