using System;
using _Game.Common;
using _Game.Gameplay.Common;

namespace _Game.Core.Ads
{
    public interface IAdsService
    {
        event Action<int> OnCountAds;

        event Action<string, MaxSdkBase.AdInfo> OnAdRevenuePaidEvent;
        event Action<string, MaxSdkBase.AdInfo, Placement> OnAdRevenueWPlacementEvent;
        void ShowRewardedVideo(Action onVideoCompleted, Placement placement);
        public event Action RewardedVideoLoaded;
        public event Action InterstitialVideoLoaded;
        bool IsAdReady(AdType type);
        void ShowInterstitialVideo(Placement placement);
        event Action<AdType, Placement, MaxSdkBase.AdInfo, AdStatus, int> OnAdImpressionStatus;
        event Action<AdType, Placement, MaxSdkBase.AdInfo, int> OnAdImpressionCustom;
    }
}