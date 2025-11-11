using System;
using UnityEngine;
using Zenject;

namespace _Game.Core.Ads.UI
{
    public class BannerSafeAreaService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private float _bannerHeight = 0f;

        public float BottomOffset => _bannerHeight;
        public float TopOffset => 0f;

        public BannerSafeAreaService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            Debug.Log("[BannerSafeArea] Service initialized");
        }

        public void Dispose() { }

        public void SetBannerHeight(float heightInPixels)
        {
            _bannerHeight = heightInPixels;

            Debug.Log($"[BannerSafeArea] Banner height: {_bannerHeight}px");

            _signalBus.Fire(new BannerSafeAreaChangedSignal
            {
                BottomOffset = _bannerHeight,
                TopOffset = 0f
            });
        }

        public void ClearBannerHeight()
        {
            SetBannerHeight(0f);
        }
    }

    public class BannerSafeAreaChangedSignal
    {
        public float BottomOffset;
        public float TopOffset;
    }

    public enum BannerPosition
    {
        Top,
        Bottom
    }
}
