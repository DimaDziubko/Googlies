using UnityEngine;
using Zenject;

namespace _Game.Core.Ads.UI
{
    public class BannerCanvasOffset : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector3 _originalPosition;
        private BannerSafeAreaService _safeAreaService;
        private SignalBus _signalBus;
        private bool _isSubscribed = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalPosition = _rectTransform.anchoredPosition3D;

            // Резолвим из контейнера
            var context = ProjectContext.Instance;
            if (context != null)
            {
                _safeAreaService = context.Container.TryResolve<BannerSafeAreaService>();
                _signalBus = context.Container.TryResolve<SignalBus>();
            }
        }

        private void Start()
        {
            // Сразу применяем текущий offset
            if (_safeAreaService != null)
            {
                ApplyOffset(_safeAreaService.BottomOffset);
            }

            // Подписываемся
            if (_signalBus != null)
            {
                _signalBus.Subscribe<BannerSafeAreaChangedSignal>(OnBannerChanged);
                _isSubscribed = true;
            }
        }

        private void OnDestroy()
        {
            if (_isSubscribed && _signalBus != null)
            {
                _signalBus.Unsubscribe<BannerSafeAreaChangedSignal>(OnBannerChanged);
            }
        }

        private void OnBannerChanged(BannerSafeAreaChangedSignal signal)
        {
            ApplyOffset(signal.BottomOffset);
        }

        private void ApplyOffset(float bannerHeightInPixels)
        {
            Canvas canvas = GetComponent<Canvas>();
            float scale = canvas != null ? canvas.scaleFactor : 1f;

            float offsetInCanvasUnits = bannerHeightInPixels / scale;

            Vector3 newPosition = _originalPosition;
            newPosition.y = _originalPosition.y + offsetInCanvasUnits;

            _rectTransform.anchoredPosition3D = newPosition;
        }
    }
}
